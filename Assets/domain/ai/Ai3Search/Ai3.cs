using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai3 : MonoBehaviour, AiInterface
{
    public int Ply = 1;
    private System.Random random = new System.Random();
    private CancellationTokenSource cancellationToken;
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        var gameState = new V10GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();
        var bestIndices = new List<int> { };
        var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
        cancellationToken = new CancellationTokenSource();

        for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
        {
            if (cancellationToken.Token.IsCancellationRequested) break;
            gameState.PlayMove(legalMoves[legalMoveIndex]);
            var value = Ai3Search.Search(gameState, Ply, cancellationToken.Token);
            gameState.UndoMove();

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

        if (bestIndices.Count == 0) throw new System.Exception("Cannot set legal move index to play");

        cancellationToken = null;

        return Task.FromResult(legalMoves[bestIndices[random.Next(0, bestIndices.Count)]]);
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
