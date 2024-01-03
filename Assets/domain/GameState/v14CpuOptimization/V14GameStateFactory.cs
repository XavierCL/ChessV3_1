public class V14GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V14GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V14GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V14GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
