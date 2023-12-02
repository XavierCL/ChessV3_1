public class V7GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V7GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V7GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V7GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
