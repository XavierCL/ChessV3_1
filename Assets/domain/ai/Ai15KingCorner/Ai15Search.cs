public static class Ai15Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai15SearchResult Search(V17GameState gameState, int depth, Ai15SearchResult alpha, Ai15SearchResult beta, Ai15SearchResult.HyperParameters hyperParameters)
  {
    if (depth <= 1) return Ai15SearchExtension.Search(gameState, alpha, beta, hyperParameters);

    var legalMoves = gameState.getLegalMoves();

    if (legalMoves.Count == 0)
    {
      var endGameState = gameState.GetGameEndState();

      if (endGameState == GameEndState.WhiteWin)
      {
        return new Ai15SearchResult(double.MaxValue, true, 1);
      }
      else if (endGameState == GameEndState.BlackWin)
      {
        return new Ai15SearchResult(double.MinValue, true, 1);
      }

      return new Ai15SearchResult(0, true, 1);
    }

    var bestSearchResult = beta;
    var allTerminalLeaves = true;
    var nodeCount = 1L;
    var sortedMoves = depth - 1 > hyperParameters.sortFromDepth ? Ai15Evaluate.SortMoves(legalMoves, gameState) : legalMoves;

    for (var legalMoveIndex = 0; legalMoveIndex < sortedMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(sortedMoves[legalMoveIndex]);
      var searchResult = Search(gameState, depth - 1, bestSearchResult, alpha, hyperParameters);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (hyperParameters.timeManagement.ShouldStop(depth)) break;

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