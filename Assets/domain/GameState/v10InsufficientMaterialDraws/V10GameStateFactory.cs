public class V10GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V10GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V10GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V10GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
