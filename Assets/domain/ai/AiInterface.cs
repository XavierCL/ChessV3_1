using System;
using System.Threading.Tasks;

public interface AiInterface
{
    Task<Move> GetMove(GameStateInterface gameState, TimeSpan remainingTime, TimeSpan increment);

    void ResetAi();
}
