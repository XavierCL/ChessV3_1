public class V11GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V11GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V11GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V11GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
