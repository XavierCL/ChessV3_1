using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Ai3 : MonoBehaviour, AiInterface
{
    public int Ply = 1;
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface referenceGameState)
    {
        var gameState = new V9GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();
        var bestIndices = new List<int> { };
        var bestValue = gameState.boardState.whiteTurn ? double.MinValue : double.MaxValue;

        for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
        {
            gameState.PlayMove(legalMoves[legalMoveIndex]);
            var value = Ai3Search.Search(gameState, Ply);
            gameState.UndoMove();

            if (value == bestValue)
            {
                bestIndices.Add(legalMoveIndex);
            }
            else if (value > bestValue && gameState.boardState.whiteTurn || value < bestValue && !gameState.boardState.whiteTurn)
            {
                bestIndices = new List<int> { legalMoveIndex };
                bestValue = value;
            }
        }

        if (bestIndices.Count == 0) throw new System.Exception("Cannot set legal move index to play");

        return Task.FromResult(legalMoves[bestIndices[random.Next(0, bestIndices.Count)]]);
    }

    public void ResetAi()
    {
    }
}
