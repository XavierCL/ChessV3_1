using System;
using System.Threading.Tasks;


public class Ai1Random : AiInterface
{
    private Random random = new Random();
    public Task<Move> GetMove(GameState gameState)
    {
        var legalMoves = gameState.getLegalMoves();
        return Task.FromResult(legalMoves[random.Next(0, legalMoves.Count)]);
    }
}
