using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai5 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    private CancellationTokenSource cancellationToken;
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai5TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

        var gameState = new V14GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1) return Task.FromResult(legalMoves[0]);

        var depth = 1;
        var bestIndicesEver = Enumerable.Range(0, legalMoves.Count).ToList();
        var bestValueEver = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;

        while (true)
        {
            var bestIndices = new List<int> { };
            var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
            var allTerminalLeaves = true;

            for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
            {
                if (legalMoveIndex <= legalMoves.Count / 2 && timeManagement.ShouldStop(depth))
                {
                    if (ShowDebugInfo)
                    {
                        Debug.Log($"Ai5 Depth: {depth}, ratio: {legalMoveIndex}/{legalMoves.Count}, Elapsed: {timeManagement.GetElapsed().TotalSeconds}, Best moves: {bestIndicesEver.Count}, Evaluation: {bestValueEver}");
                    }
                    break;
                }

                gameState.PlayMove(legalMoves[legalMoveIndex]);
                var searchResult = Ai5Search.Search(gameState, depth);
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

            if (timeManagement.ShouldStop(depth)) break;

            bestIndicesEver = bestIndices;
            bestValueEver = bestValue;

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

            ++depth;
        }

        if (bestIndicesEver.Count == 0) throw new System.Exception("Cannot set legal move index to play");

        cancellationToken = null;

        return Task.FromResult(legalMoves[bestIndicesEver[random.Next(0, bestIndicesEver.Count)]]);
    }

    public string GetStats()
    {
        return "";
    }

    public void ResetAi()
    {
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken = null;
        }
    }
}
