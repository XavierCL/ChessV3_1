public static class Ai8SearchExtension
{
  public static Ai8SearchResult Search(V14GameState gameState, Ai8SearchResult.Hyperparameters hyperparameters)
  {
    var lastMove = gameState.history[^1];
    if (lastMove.killed == null || !hyperparameters.searchExtensions) return Ai8Evaluate.Evaluate(gameState);

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

    var bestSearchResult = Ai8Evaluate.Evaluate(gameState);
    var allowedTarget = lastMove.killed.position.index;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, hyperparameters);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (hyperparameters.timeManagement.ShouldStop(1)) break;

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

    return bestSearchResult.SetParentSearch(false, nodeCount);
  }
}