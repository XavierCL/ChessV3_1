using System.Threading.Tasks;
using UnityEngine;

public class Ai1Random : MonoBehaviour, AiInterface
{
    public Task<Move> GetMove(GameState gameState)
    {
        var legalMoves = gameState.getLegalMoves();
        return Task.FromResult(legalMoves[Random.Range(0, legalMoves.Count - 1)]);
    }
}
