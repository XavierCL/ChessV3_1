public class PieceIdGameStateFactory : GameStateFactoryInterface
{
  private GameStateFactoryInterface underlying = new V17GameStateFactory();

  public override GameStateInterface StartingPosition()
  {
    return new PieceIdGameStateAdapter(underlying);
  }

  public override GameStateInterface FromGameState(GameStateInterface gameState)
  {
    return new PieceIdGameStateAdapter(underlying, gameState);
  }

  public override GameStateInterface FromFen(string fen)
  {
    return new PieceIdGameStateAdapter(underlying, fen);
  }
}
