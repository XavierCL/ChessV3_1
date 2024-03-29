using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateChecker : MonoBehaviour
{
    public int Ply = 1;
    public bool showFirstPly = false;
    public bool OnlyFirstPosition = false;
    public bool OnlyFirstGameState = false;
    public bool PlayMoveLast = false;

    void Start()
    {
        var version1Counts = new List<long>();
        var version2Counts = new List<long>();
        var version1Time = 0.0;
        var version2Time = 0.0;
        var factory1 = new V17GameStateFactory();
        var factory2 = new V16GameStateFactory();

        var startingPositions = StartingPositions();
        if (OnlyFirstPosition)
        {
            startingPositions = new List<GameStateInterface> { startingPositions[0] };
        }

        for (var startingPositionIndex = 0; startingPositionIndex < startingPositions.Count; ++startingPositionIndex)
        {
            var startTime = DateTime.UtcNow;
            version1Counts.Add(CountLegalMoves(factory1.FromGameState(startingPositions[startingPositionIndex]), Ply, showFirstPly));
            version1Time += (DateTime.UtcNow - startTime).TotalMilliseconds;

            if (!OnlyFirstGameState)
            {
                startTime = DateTime.UtcNow;
                version2Counts.Add(CountLegalMoves(factory2.FromGameState(startingPositions[startingPositionIndex]), Ply, showFirstPly));
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

            if (PlayMoveLast)
            {
                for (var moveIndex = 0; moveIndex < legalMoves.Count; ++moveIndex)
                {
                    gameState.PlayMove(legalMoves[moveIndex]);
                    // Fake evaluation
                    gameState.UndoMove();
                }
            }

            return legalMoves.Count;
        }

        if (showPly)
        {
            legalMoves = legalMoves.OrderBy(move => move.ToString()).ToList();
        }

        var firstPlyCounts = new List<string>();

        long count = 0;
        for (var moveIndex = 0; moveIndex < legalMoves.Count; ++moveIndex)
        {
            gameState.PlayMove(legalMoves[moveIndex]);
            var moveCount = CountLegalMoves(gameState, ply - 1, false);
            count += moveCount;
            gameState.UndoMove();

            if (showPly)
            {
                firstPlyCounts.Add($"{legalMoves[moveIndex]}: {moveCount}");
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
            new V17GameStateFactory().StartingPosition(),
            new V17GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP2/RNB2KRr w -"),
            new V17GameStateFactory().FromFen("rnbqkb1r/pppp1p1p/5np1/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R b KQkq"),
            new V17GameStateFactory().FromFen("r3k2r/p1ppqpb1/Bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPB1PPP/R3K2R b KQkq -"),
            new V17GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP1p/RNB1K1R1 w -"),
            new V17GameStateFactory().FromFen("1r6/4b3/p6P/k7/2p5/8/5r2/7K b -"),
            new V17GameStateFactory().FromFen("4b2k/3p1p1p/3P1P1P/8/8/p1p1p3/P1P1P3/K2B4 w -"),
            new V17GameStateFactory().FromFen("rnbqk2r/pp2bpp1/5n1p/P2pp3/8/NP2PNPB/1pPPQP1P/R3K2R b KQkq"),
            new V17GameStateFactory().FromFen("8/p1k2p2/1pr2p2/6p1/8/1P1R4/P1P3PP/K7 w -"),
        };
    }
}
