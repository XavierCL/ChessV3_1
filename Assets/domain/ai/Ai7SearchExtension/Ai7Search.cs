public static class Ai7Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai7SearchResult Search(V14GameState gameState, int depth)
  {
    if (depth <= 1) return Ai7SearchExtension.Search(gameState);

    var legalMoves = gameState.getLegalMoves();
    var endGameState = gameState.GetGameEndState();

    if (endGameState != GameEndState.Ongoing) return Ai7Evaluate.Evaluate(gameState);

    var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
    var allTerminalLeaves = true;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1);
      gameState.UndoMove();

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
      {
        bestValue = searchResult.value;
      }

      // Return early if the best outcome can be achieved
      if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
      {
        return new Ai7SearchResult(bestValue, true);
      }
    }

    return new Ai7SearchResult(bestValue, allTerminalLeaves);
  }
}