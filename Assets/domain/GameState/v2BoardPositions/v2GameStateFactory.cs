public class V2GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V2GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V2GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V2GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
