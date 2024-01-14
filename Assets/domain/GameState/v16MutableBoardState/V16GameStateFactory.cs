public class V16GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V16GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V16GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V16GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
