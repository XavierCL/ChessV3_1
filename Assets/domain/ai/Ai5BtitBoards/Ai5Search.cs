public static class Ai5Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static double Search(V12GameState gameState, int depth, Ai5TimeManagement timeManagement)
  {
    var legalMoves = gameState.getLegalMoves();
    if (depth <= 1 && legalMoves.Count != 1) return Ai5Evaluate.Evaluate(gameState);

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

    var bestValue = gameState.boardState.whiteTurn ? double.MinValue : double.MaxValue;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      if (timeManagement.ShouldStop()) return bestValue;
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var value = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, timeManagement);
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