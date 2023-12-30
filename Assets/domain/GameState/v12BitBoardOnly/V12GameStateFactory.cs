public class V12GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V12GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V12GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V12GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
