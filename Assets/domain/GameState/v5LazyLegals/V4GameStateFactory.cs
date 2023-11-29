public class V5GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V5GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V5GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V5GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
