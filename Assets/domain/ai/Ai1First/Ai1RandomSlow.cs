using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class Ai1RandomSlow : MonoBehaviour, AiInterface
{
    public int DelayMs;
    private System.Random random = new System.Random();
    private CancellationTokenSource cancellationTokenSource;
    public async Task<Move> GetMove(GameStateInterface gameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationTokenSource = new CancellationTokenSource();
        try
        {
            var legalMoves = gameState.getLegalMoves();
            await Task.Delay(DelayMs, cancellationTokenSource.Token);
            cancellationTokenSource = null;
            return legalMoves[random.Next(0, legalMoves.Count)];
        }
        catch
        {
            // Delay cancelled, return no move
            return null;
        }
    }

    public void ResetAi()
    {
        if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
        cancellationTokenSource = null;
    }
}
