public static class Ai3Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static double Search(V9GameState gameState, int depth)
  {
    if (depth == 1) return Ai3Evaluate.Evaluate(gameState);

    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return double.MaxValue;
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return double.MinValue;
    }
    else if (endGameState == GameEndState.Draw)
    {
      return 0;
    }

    var legalMoves = gameState.getLegalMoves();
    var bestValue = gameState.boardState.whiteTurn ? double.MinValue : double.MaxValue;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var value = Search(gameState, depth - 1);
      gameState.UndoMove();

      if (value > bestValue && gameState.boardState.whiteTurn || value < bestValue && !gameState.boardState.whiteTurn)
      {
        bestValue = value;
      }

      // Return early if the best outcome can be achieved
      if (bestValue == double.MaxValue && gameState.boardState.whiteTurn || bestValue == double.MinValue && !gameState.boardState.whiteTurn)
      {
        return bestValue;
      }
    }

    return bestValue;
  }
}