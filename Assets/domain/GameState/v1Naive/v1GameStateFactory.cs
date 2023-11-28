public class V1GameStateFactory : GameStateFactoryInterface
{
    public override GameStateInterface StartingPosition()
    {
        return new V1GameState();
    }

    public override GameStateInterface FromGameState(GameStateInterface gameState)
    {
        return new V1GameState(gameState);
    }

    public override GameStateInterface FromFen(string fen)
    {
        return new V1GameState(FenToPiecePositions(fen), FenToWhite(fen), FenToCastle(fen));
    }
}
