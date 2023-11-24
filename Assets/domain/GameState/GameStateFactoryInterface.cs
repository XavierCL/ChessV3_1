public interface GameStateFactoryInterface
{
    public GameStateInterface StartingPosition();
    public GameStateInterface FromGameState(GameStateInterface gameState);
}
