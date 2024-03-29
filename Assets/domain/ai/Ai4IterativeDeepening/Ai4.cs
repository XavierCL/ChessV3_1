using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai4 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    private CancellationTokenSource cancellationToken;
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai4TimeManagement(remainingTime, increment, cancellationToken.Token);

        var gameState = new V11GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1) return Task.FromResult(legalMoves[0]);

        var depth = 1;
        var bestIndicesEver = Enumerable.Range(0, legalMoves.Count).ToList();

        while (!timeManagement.ShouldStop())
        {
            var bestIndices = new List<int> { };
            var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;

            for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
            {
                gameState.PlayMove(legalMoves[legalMoveIndex]);
                var value = Ai4Search.Search(gameState, depth, timeManagement);
                gameState.UndoMove();

                if (timeManagement.ShouldStop())
                {
                    if (ShowDebugInfo)
                    {
                        Debug.Log($"Ai4 Depth: {depth}, ratio: {legalMoveIndex}/{legalMoves.Count}, Elapsed: {timeManagement.GetElapsed().TotalSeconds}");
                    }
                    break;
                }

                if (value == bestValue)
                {
                    bestIndices.Add(legalMoveIndex);
                }
                else if (value > bestValue && gameState.boardState.WhiteTurn || value < bestValue && !gameState.boardState.WhiteTurn)
                {
                    bestIndices = new List<int> { legalMoveIndex };
                    bestValue = value;
                }
            }

            if (timeManagement.ShouldStop()) break;

            bestIndicesEver = bestIndices;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
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

    public void ResetStats()
    {
    }
}
