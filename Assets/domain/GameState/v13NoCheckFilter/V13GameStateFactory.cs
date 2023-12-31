public class V13GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V13GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V13GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V13GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
