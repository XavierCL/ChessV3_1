public class V1GameStateFactory : GameStateFactoryInterface
{
    public GameStateInterface StartingPosition()
    {
        return new V1GameState();
    }

    public GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V1GameState(gameState);
    }
}
