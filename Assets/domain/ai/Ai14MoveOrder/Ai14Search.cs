public static class Ai14Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai14SearchResult Search(V14GameState gameState, int depth, Ai14SearchResult alpha, Ai14SearchResult beta, Ai14SearchResult.HyperParameters hyperParameters)
  {
    if (depth <= 1) return Ai14SearchExtension.Search(gameState, alpha, beta, hyperParameters);

    var legalMoves = gameState.getLegalMoves();

    if (legalMoves.Count == 0)
    {
      var endGameState = gameState.GetGameEndState();

      if (endGameState == GameEndState.WhiteWin)
      {
        return new Ai14SearchResult(double.MaxValue, true, 1);
      }
      else if (endGameState == GameEndState.BlackWin)
      {
        return new Ai14SearchResult(double.MinValue, true, 1);
      }

      return new Ai14SearchResult(0, true, 1);
    }

    var bestSearchResult = beta;
    var allTerminalLeaves = true;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
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