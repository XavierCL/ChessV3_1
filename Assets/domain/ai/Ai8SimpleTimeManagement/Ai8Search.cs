public static class Ai8Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai8SearchResult Search(V14GameState gameState, int depth, Ai8TimeManagement timeManagement)
  {
    if (depth <= 1) return Ai8SearchExtension.Search(gameState, timeManagement);

    var legalMoves = gameState.getLegalMoves();
    var endGameState = gameState.GetGameEndState();

    if (endGameState != GameEndState.Ongoing) return Ai8Evaluate.Evaluate(gameState);

    var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
    var allTerminalLeaves = true;
    var nodeCount = 0L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
      {
        bestValue = searchResult.value;
      }

      // Return early if the best outcome can be achieved
      if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
      {
        return new Ai8SearchResult(bestValue, true, nodeCount);
      }
    }

    return new Ai8SearchResult(bestValue, allTerminalLeaves, nodeCount);
  }
}