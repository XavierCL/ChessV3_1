using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai7 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;

    private readonly System.Random random = new System.Random();

    private CancellationTokenSource cancellationToken;
    private V14GameState ownGameState;

    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai7TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

        if (ownGameState == null)
        {
            ownGameState = new V14GameState(referenceGameState);
        }
        else
        {
            while (ownGameState.history.Count < referenceGameState.history.Count)
            {
                ownGameState.PlayMove(new Move(referenceGameState.history[^(referenceGameState.history.Count - ownGameState.history.Count)]));
            }
        }

        var gameState = ownGameState;

        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1) return Task.FromResult(legalMoves[0]);

        var depth = 1;
        var bestIndicesEver = Enumerable.Range(0, legalMoves.Count).ToList();
        var bestValueEver = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
        var lastCurrentMoveIndex = 0;
        var nodeCount = 0L;

        while (true)
        {
            var bestIndices = new List<int> { };
            var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
            var allTerminalLeaves = true;

            for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
            {
                if (legalMoveIndex <= 4 * legalMoves.Count / 5 && timeManagement.ShouldStop(depth))
                {
                    break;
                }

                lastCurrentMoveIndex = legalMoveIndex + 1;

                gameState.PlayMove(legalMoves[legalMoveIndex]);
                var searchResult = Ai7Search.Search(gameState, depth);
                nodeCount += searchResult.nodeCount;
                gameState.UndoMove();

                allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

                if (searchResult.value == bestValue)
                {
                    bestIndices.Add(legalMoveIndex);
                }
                else if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
                {
                    bestIndices = new List<int> { legalMoveIndex };
                    bestValue = searchResult.value;
                }
            }

            if (lastCurrentMoveIndex < legalMoves.Count) break;

            bestIndicesEver = bestIndices;
            bestValueEver = bestValue;
            ++depth;
            lastCurrentMoveIndex = 0;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
            {
                break;
            }

            // Don't go deeper if the tree has only reached terminal leaves. Useful in case of draws.
            if (allTerminalLeaves)
            {
                break;
            }
        }

        if (ShowDebugInfo)
        {
            Debug.Log($"Ai7 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodeCount}, Elapsed: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, Best moves: {bestIndicesEver.Count}, Evaluation: {bestValueEver}");
        }

        if (bestIndicesEver.Count == 0)
        {
            throw new System.Exception("Cannot set legal move index to play");
        }

        cancellationToken = null;

        return Task.FromResult(legalMoves[bestIndicesEver[random.Next(0, bestIndicesEver.Count)]]);
    }

    public void ResetAi()
    {
        ownGameState = null;
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken = null;
        }
    }

    public string GetStats()
    {
        return "";
    }
}
