using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Ai2 : MonoBehaviour, AiInterface
{
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        var gameState = new V9GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();
        var bestIndices = new List<int> { };
        var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;

        for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
        {
            gameState.PlayMove(legalMoves[legalMoveIndex]);
            var value = Ai2Evaluate.Evaluate(gameState);
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

        return Task.FromResult(legalMoves[bestIndices[random.Next(0, bestIndices.Count)]]);
    }

    public string GetStats()
    {
        return "";
    }

    public void ResetAi()
    {
    }
}
