public class V8GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V8GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V8GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V8GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
