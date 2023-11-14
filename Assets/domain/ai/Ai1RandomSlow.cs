using System;
using System.Threading.Tasks;


public class Ai1RandomSlow : AiInterface
{
    private Random random = new Random();
    public async Task<Move> GetMove(GameState gameState)
    {
        var legalMoves = gameState.getLegalMoves();
        await Task.Delay(200000);
        return legalMoves[random.Next(0, legalMoves.Count)];
    }
}
