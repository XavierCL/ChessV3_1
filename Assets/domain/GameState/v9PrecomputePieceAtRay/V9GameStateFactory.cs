public class V9GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V9GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V9GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V9GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
