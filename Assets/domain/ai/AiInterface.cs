using System.Threading.Tasks;

public interface AiInterface
{
    Task<Move> GetMove(GameState gameState);

    void ResetAi();
}
