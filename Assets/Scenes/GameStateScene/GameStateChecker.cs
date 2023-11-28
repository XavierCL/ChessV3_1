using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateChecker : MonoBehaviour
{
    public int Ply = 1;
    public bool showFirstPly = false;

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
            version1Counts.Add(CountLegalMoves(factory1.FromGameState(startingPosition), Ply, showFirstPly));
            version1Time += (DateTime.UtcNow - startTime).TotalMilliseconds;

            // startTime = DateTime.UtcNow;
            // version2Counts.Add(CountLegalMoves(factory2.FromGameState(startingPosition), Ply, false));
            // version2Time += (DateTime.UtcNow - startTime).TotalMilliseconds;
        }

        Debug.Log(("Time 1", version1Time / 1000));
        Debug.Log(("Time 2", version2Time / 1000));
        Debug.Log("Counts 1:" + string.Join(", ", version1Counts));
        Debug.Log("Counts 2:" + string.Join(", ", version2Counts));

        if (!version1Counts.SequenceEqual(version2Counts))
        {
            throw new Exception("Not equal");
        }
    }

    private long CountLegalMoves(GameStateInterface gameState, int ply, bool showPly)
    {
        if (gameState.GetGameEndState() != GameEndState.Ongoing) return 0;

        var legalMoves = gameState.getLegalMoves().OrderBy(move => move.ToString()).ToList();

        if (showPly)
        {
            Debug.Log($"First ply: {legalMoves.Count}");
        }

        if (ply <= 1) return legalMoves.Count;

        long count = 0;
        foreach (var move in legalMoves)
        {
            gameState.PlayMove(move);
            var moveCount = CountLegalMoves(gameState, ply - 1, false);
            count += moveCount;
            gameState.UndoMove();

            if (showPly)
            {
                Debug.Log($"{move}: {moveCount}");
            }
        }
        return count;
    }

    private List<GameStateInterface> StartingPositions()
    {
        return new List<GameStateInterface> {
            new V1GameStateFactory().StartingPosition(),
            new V1GameStateFactory().FromFen("rnbqkb1r/pppp1p1p/5np1/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R b KQkq"),
            new V1GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP1p/RNB1K1R1 w -"),
        };
    }
}
