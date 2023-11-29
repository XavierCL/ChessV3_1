public class V3GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V3GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V3GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V3GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
