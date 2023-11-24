using System.Threading.Tasks;
using UnityEngine;


public class Ai1Random : MonoBehaviour, AiInterface
{
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface gameState)
    {
        var legalMoves = gameState.getLegalMoves();
        return Task.FromResult(legalMoves[random.Next(0, legalMoves.Count)]);
    }

    public void ResetAi()
    {
    }
}
