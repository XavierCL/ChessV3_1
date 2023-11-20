using System.Threading.Tasks;
using UnityEngine;


public class Ai1FirstMove : MonoBehaviour, AiInterface
{
    public Task<Move> GetMove(GameState gameState)
    {
        return Task.FromResult(gameState.getLegalMoves()[0]);
    }

    public void ResetAi()
    {
    }
}
