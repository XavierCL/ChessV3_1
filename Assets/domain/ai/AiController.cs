using System.Threading.Tasks;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public AiInterface Ai1 = new Ai1RandomSlow();

    public AiInterface Ai2 = new Ai1RandomSlow();

    public async Task<Move> GetMove(GameState gameState)
    {
        return await Task.Run(async () =>
        {
            if (gameState.whiteTurn) return await Ai1.GetMove(gameState);
            return await Ai2.GetMove(gameState);
        });
    }
}
