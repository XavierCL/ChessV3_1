using System;
using System.Threading.Tasks;
using UnityEngine;


public class Ai1Random : MonoBehaviour, AiInterface
{
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface gameState, TimeSpan remainingTime, TimeSpan increment)
    {
        var legalMoves = gameState.getLegalMoves();
        return Task.FromResult(legalMoves[random.Next(0, legalMoves.Count)]);
    }

    public string GetStats()
    {
        return "";
    }

    public void ResetAi()
    {
    }
}
