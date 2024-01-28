public static class Ai16Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai16SearchResult Search(V17GameState gameState, int depth, Ai16SearchResult alpha, Ai16SearchResult beta, Ai16SearchResult.HyperParameters hyperParameters)
  {
    if (depth <= 1) return Ai16SearchExtension.Search(gameState, alpha, beta, hyperParameters);

    var legalMoves = gameState.getLegalMoves();

    if (legalMoves.Count == 0)
    {
      var endGameState = gameState.GetGameEndState();

      if (endGameState == GameEndState.WhiteWin)
      {
        return new Ai16SearchResult(double.MaxValue, Ai16Evaluate.MAX_EVALUATION, true, 1, false);
      }
      else if (endGameState == GameEndState.BlackWin)
      {
        return new Ai16SearchResult(double.MinValue, -Ai16Evaluate.MAX_EVALUATION, true, 1, false);
      }

      return new Ai16SearchResult(0, 0, true, 1, false);
    }

    var bestSearchResult = beta;
    var allTerminalLeaves = true;
    var nodeCount = 1L;
    var sum = 0.0;
    var sortedMoves = depth - 1 > hyperParameters.sortFromDepth ? Ai16Evaluate.SortMoves(legalMoves, gameState) : legalMoves;

    for (var legalMoveIndex = 0; legalMoveIndex < sortedMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(sortedMoves[legalMoveIndex]);
      var searchResult = Search(gameState, depth - 1, bestSearchResult, alpha, hyperParameters);
      nodeCount += searchResult.nodeCount;
      sum += searchResult.sum;
      gameState.UndoMove();

      if (hyperParameters.timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          return searchResult.SetParentSearch(sum * sortedMoves.Count / (legalMoveIndex + 1), true, nodeCount);
        }

        // If search result would be returned, it'd be worst than the previous move. Return early.
        if (!alpha.IsBetterThan(searchResult, gameState))
        {
          return searchResult.SetAlphaSkiped(sum * sortedMoves.Count / (legalMoveIndex + 1), nodeCount);
        }

        bestSearchResult = searchResult;
      }
    }

    return bestSearchResult.SetParentSearch(sum, allTerminalLeaves, nodeCount);
  }
}