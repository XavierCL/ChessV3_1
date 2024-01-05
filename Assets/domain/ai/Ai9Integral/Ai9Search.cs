public static class Ai9Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai9SearchResult Search(V14GameState gameState, int depth, Ai9TimeManagement timeManagement)
  {
    if (depth <= 1) return Ai9Evaluate.Evaluate(gameState);

    var legalMoves = gameState.getLegalMoves();
    var endGameState = gameState.GetGameEndState();
    var idleEvaluation = Ai9Evaluate.Evaluate(gameState);

    if (endGameState != GameEndState.Ongoing) return idleEvaluation;

    var bestSearchResult = idleEvaluation.SetValue(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue);
    var allTerminalLeaves = true;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Debugging
        gameState.PlayMove(legalMoves[legalMoveIndex]);
        Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, timeManagement);
        gameState.UndoMove();

        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          return new Ai9SearchResult(
            idleEvaluation,
            searchResult,
            nodeCount
          );
        }

        bestSearchResult = searchResult;
      }
    }

    return new Ai9SearchResult(idleEvaluation, bestSearchResult, allTerminalLeaves, nodeCount);
  }
}