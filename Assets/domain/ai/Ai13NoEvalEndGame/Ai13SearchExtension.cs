public static class Ai13SearchExtension
{
  public static Ai13SearchResult Search(V16GameState gameState, Ai13SearchResult alpha, Ai13SearchResult beta, Ai13SearchResult.Hyperparameters hyperParameters)
  {
    var lastMove = gameState.History[^1];
    if (lastMove.killed == null || !hyperParameters.searchExtensions) return Ai13Evaluate.Evaluate(gameState);

    var legalMoves = gameState.getLegalMoves();

    if (legalMoves.Count == 0)
    {
      var endGameState = gameState.GetGameEndState();

      if (endGameState == GameEndState.WhiteWin)
      {
        return new Ai13SearchResult(double.MaxValue, true, 1);
      }
      else if (endGameState == GameEndState.BlackWin)
      {
        return new Ai13SearchResult(double.MinValue, true, 1);
      }

      return new Ai13SearchResult(0, true, 1);
    }

    var idleEvaluation = Ai13Evaluate.Evaluate(gameState);

    // If idle would be returned, it'd already be worst than the best parent move. Return early.
    if (!alpha.IsBetterThan(idleEvaluation, gameState)) return idleEvaluation;

    // Select best between best grand parent and idle as starting move, since both are possible.
    var bestSearchResult = idleEvaluation.IsBetterThan(beta, gameState) ? idleEvaluation : beta;

    var allowedTarget = lastMove.killed.position.index;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, bestSearchResult, alpha, hyperParameters);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (hyperParameters.timeManagement.ShouldStop(1)) break;

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

    return bestSearchResult.SetParentSearch(false, nodeCount);
  }
}