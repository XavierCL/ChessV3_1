public class V17GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V17GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V17GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V17GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
