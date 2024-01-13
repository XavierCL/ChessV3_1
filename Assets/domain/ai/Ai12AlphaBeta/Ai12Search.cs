public static class Ai12Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai12SearchResult Search(V14GameState gameState, int depth, Ai12SearchResult alpha, Ai12SearchResult beta, Ai12TimeManagement timeManagement)
  {
    if (depth <= 1) return Ai12SearchExtension.Search(gameState, alpha, beta, timeManagement);

    var legalMoves = gameState.getLegalMoves();

    if (legalMoves.Count == 0)
    {
      return Ai12Evaluate.Evaluate(gameState);
    }

    var bestSearchResult = beta;
    var allTerminalLeaves = true;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, bestSearchResult, alpha, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          return searchResult.SetParentSearch(true, nodeCount);
        }

        // If search result would be returned, it'd be worst than the previous move. Return early.
        if (!alpha.IsBetterThan(searchResult, gameState))
        {
          return searchResult;
        }

        bestSearchResult = searchResult;
      }
    }

    return bestSearchResult.SetParentSearch(allTerminalLeaves, nodeCount);
  }
}