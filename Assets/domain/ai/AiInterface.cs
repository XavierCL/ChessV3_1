using System.Threading.Tasks;

public interface AiInterface
{
    Task<Move> GetMove(GameStateInterface gameState);

    void ResetAi();
}
