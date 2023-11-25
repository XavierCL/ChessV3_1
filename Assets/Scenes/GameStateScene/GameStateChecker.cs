using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateChecker : MonoBehaviour
{
    public int Ply = 1;

    void Start()
    {
        var version1Counts = new List<long>();
        var version2Counts = new List<long>();
        var version1Time = 0.0;
        var version2Time = 0.0;
        var factory1 = new V1GameStateFactory();
        var factory2 = new V1GameStateFactory();

        foreach (var startingPosition in StartingPositions())
        {
            var startTime = DateTime.UtcNow;
            version1Counts.Add(CountLegalMoves(factory1.FromGameState(startingPosition), Ply));
            version1Time += (DateTime.UtcNow - startTime).TotalMilliseconds;

            startTime = DateTime.UtcNow;
            version2Counts.Add(CountLegalMoves(factory2.FromGameState(startingPosition), Ply));
            version2Time += (DateTime.UtcNow - startTime).TotalMilliseconds;
        }

        Debug.Log(("Time 1", version1Time / 1000));
        Debug.Log(("Time 2", version2Time / 1000));
        Debug.Log("Counts 1:" + string.Join(", ", version1Counts));
        Debug.Log("Counts 2:" + string.Join(", ", version2Counts));

        if (!version1Counts.SequenceEqual(version2Counts))
        {
            throw new System.Exception("Not equal");
        }
    }

    private long CountLegalMoves(GameStateInterface gameState, int ply)
    {
        var legalMoves = gameState.getLegalMoves();

        if (ply <= 1) return legalMoves.Count;

        long count = 0;
        foreach (var move in legalMoves)
        {
            gameState.PlayMove(move);
            count += CountLegalMoves(gameState, ply - 1);
            gameState.UndoMove();
        }
        return count;
    }

    private List<GameStateInterface> StartingPositions()
    {
        return new List<GameStateInterface> { new V1GameStateFactory().StartingPosition() };
    }
}
