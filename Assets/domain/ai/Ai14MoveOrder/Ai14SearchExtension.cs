public static class Ai14SearchExtension
{
  public static Ai14SearchResult Search(V16GameState gameState, Ai14SearchResult alpha, Ai14SearchResult beta, Ai14SearchResult.HyperParameters hyperParameters)
  {
    var lastMove = gameState.History[^1];
    if (lastMove.killed == null || !hyperParameters.searchExtensions) return Ai14Evaluate.Evaluate(gameState);

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

    var idleEvaluation = Ai14Evaluate.Evaluate(gameState);

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