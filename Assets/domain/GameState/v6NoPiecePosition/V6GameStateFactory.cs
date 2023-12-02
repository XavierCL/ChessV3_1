public class V6GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V6GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V6GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V6GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
