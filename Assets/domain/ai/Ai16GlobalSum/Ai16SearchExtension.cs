public static class Ai16SearchExtension
{
  public static Ai16SearchResult Search(V17GameState gameState, Ai16SearchResult alpha, Ai16SearchResult beta, Ai16SearchResult.HyperParameters hyperParameters)
  {
    var lastMove = gameState.History[^1];
    if (lastMove.killed == null || !hyperParameters.searchExtensions) return Ai16Evaluate.Evaluate(gameState, hyperParameters);

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

    var idleEvaluation = Ai16Evaluate.Evaluate(gameState, hyperParameters);

    // If idle would be returned, it'd already be worst than the best parent move. Return early.
    if (!alpha.IsBetterThan(idleEvaluation, gameState)) return idleEvaluation;

    // Select best between best grand parent and idle as starting move, since both are possible.
    var bestSearchResult = idleEvaluation.IsBetterThan(beta, gameState) ? idleEvaluation : beta;

    var allowedTarget = lastMove.killed.position.index;
    var nodeCount = 1L;
    var sum = idleEvaluation.sum;

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
          return searchResult.SetParentSearch(sum, true, nodeCount);
        }

        // If search result would be returned, it'd be worst than the previous move. Return early.
        if (!alpha.IsBetterThan(searchResult, gameState))
        {
          return searchResult.SetAlphaSkiped(sum, nodeCount);
        }

        bestSearchResult = searchResult;
      }

      // Assuming the legal moves are preordered by smallest to largest capturer
      break;
    }

    return bestSearchResult.SetParentSearch(sum, false, nodeCount);
  }
}