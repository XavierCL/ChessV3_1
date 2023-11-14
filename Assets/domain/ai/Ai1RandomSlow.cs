using System;
using System.Threading.Tasks;
using UnityEngine;


public class Ai1RandomSlow : MonoBehaviour, AiInterface
{
    public int DelayMs;
    private System.Random random = new System.Random();
    public async Task<Move> GetMove(GameState gameState)
    {
        var legalMoves = gameState.getLegalMoves();
        await Task.Delay(DelayMs);
        return legalMoves[random.Next(0, legalMoves.Count)];
    }
}
