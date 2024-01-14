public class V15GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V15GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V15GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V15GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
