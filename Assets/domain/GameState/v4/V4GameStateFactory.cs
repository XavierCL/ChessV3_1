public class V4GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V4GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V4GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V4GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
