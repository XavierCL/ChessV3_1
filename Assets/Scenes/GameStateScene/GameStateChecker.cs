using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class GameStateChecker : MonoBehaviour
{
    public int Ply = 1;
    public bool showFirstPly = false;
    public bool OnlyLastPosition = false;
    public bool OnlyFirstGameState = false;

    void Start()
    {
        var version1Counts = new List<long>();
        var version2Counts = new List<long>();
        var version1Time = 0.0;
        var version2Time = 0.0;
        var factory1 = new V8GameStateFactory();
        var factory2 = new V7GameStateFactory();

        var startingPositions = StartingPositions();
        if (OnlyLastPosition)
        {
            startingPositions = new List<GameStateInterface> { startingPositions[^1] };
        }

        foreach (var startingPosition in startingPositions)
        {
            var startTime = DateTime.UtcNow;
            version1Counts.Add(CountLegalMoves(factory1.FromGameState(startingPosition), Ply, showFirstPly));
            version1Time += (DateTime.UtcNow - startTime).TotalMilliseconds;

            if (!OnlyFirstGameState)
            {
                startTime = DateTime.UtcNow;
                version2Counts.Add(CountLegalMoves(factory2.FromGameState(startingPosition), Ply, false));
                version2Time += (DateTime.UtcNow - startTime).TotalMilliseconds;
            }
        }

        Debug.Log(("Time 1", version1Time / 1000));
        Debug.Log(("Time 2", version2Time / 1000));
        Debug.Log("Counts 1:" + string.Join(", ", version1Counts));
        Debug.Log("Counts 2:" + string.Join(", ", version2Counts));

        if (!OnlyFirstGameState && !version1Counts.SequenceEqual(version2Counts))
        {
            throw new Exception("Not equal");
        }
    }

    private long CountLegalMoves(GameStateInterface gameState, int ply, bool showPly)
    {
        if (gameState.GetGameEndState() != GameEndState.Ongoing) return 0;

        var legalMoves = gameState.getLegalMoves();

        if (showPly)
        {
            Debug.Log($"First ply: {legalMoves.Count}");
        }

        if (ply <= 1)
        {
            if (showPly)
            {
                Debug.Log(string.Join("\n", legalMoves.OrderBy(move => move.ToString()).Select(move => $"{move}: {1}")));
            }

            return legalMoves.Count;
        }

        if (showPly)
        {
            legalMoves = legalMoves.OrderBy(move => move.ToString()).ToList();
        }

        var firstPlyCounts = new List<string>();

        long count = 0;
        foreach (var move in legalMoves)
        {
            gameState.PlayMove(move);
            var moveCount = CountLegalMoves(gameState, ply - 1, false);
            count += moveCount;
            gameState.UndoMove();

            if (showPly)
            {
                firstPlyCounts.Add($"{move}: {moveCount}");
            }
        }

        if (showPly)
        {
            Debug.Log(string.Join("\n", firstPlyCounts));
        }

        return count;
    }

    private List<GameStateInterface> StartingPositions()
    {
        return new List<GameStateInterface> {
            new V1GameStateFactory().StartingPosition(),
            new V1GameStateFactory().FromFen("rnbqkb1r/pppp1p1p/5np1/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R b KQkq"),
            new V1GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP1p/RNB1K1R1 w -"),
            new V1GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP2/RNB2KRr w -"),
            new V1GameStateFactory().FromFen("1r6/4b3/p6P/k7/2p5/8/5r2/7K b -"),
            new V1GameStateFactory().FromFen("1n2r3/4k2N/P1p5/2P1p1p1/2P1p1Pr/1pb5/1B2K3/4R3 w -"),
            new V1GameStateFactory().FromFen("8/3B4/8/1R3K1p/7k/2p5/p7/8 w -"),
            new V1GameStateFactory().FromFen("K3k3/7P/3q4/7R/3r4/8/8/8 b -"),
            new V1GameStateFactory().FromFen("8/8/6K1/3R4/7k/8/p7/8 w -"),
            new V1GameStateFactory().FromFen("rnbqk2r/pp2bpp1/5n1p/P2pp3/8/1Pp1PNPB/1BPPQP1P/RN2K2R b KQkq"),
            new V1GameStateFactory().FromFen("rnbqk2r/pp2bpp1/5n1p/P2pp3/8/NP2PNPB/1pPPQP1P/R3K2R b KQkq"),
        };
    }
}
