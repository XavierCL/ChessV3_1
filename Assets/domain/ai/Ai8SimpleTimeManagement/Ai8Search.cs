public static class Ai8Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai8SearchResult Search(V14GameState gameState, int depth, Ai8SearchResult.Hyperparameters hyperparameters)
  {
    if (depth <= 1) return Ai8SearchExtension.Search(gameState, hyperparameters);

    var legalMoves = gameState.getLegalMoves();

    if (legalMoves.Count == 0)
    {
      var endGameState = gameState.GetGameEndState();

      if (endGameState == GameEndState.WhiteWin)
      {
        return new Ai8SearchResult(double.MaxValue, true, 1);
      }
      else if (endGameState == GameEndState.BlackWin)
      {
        return new Ai8SearchResult(double.MinValue, true, 1);
      }

      return new Ai8SearchResult(0, true, 1);
    }

    var bestSearchResult = new Ai8SearchResult(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue, false, 0);
    var allTerminalLeaves = true;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, depth - 1, hyperparameters);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (hyperparameters.timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          return searchResult.SetParentSearch(true, nodeCount);
        }

        bestSearchResult = searchResult;
      }
    }

    return bestSearchResult.SetParentSearch(allTerminalLeaves, nodeCount);
  }
}