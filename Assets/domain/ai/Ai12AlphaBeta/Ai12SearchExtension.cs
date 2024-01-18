public static class Ai12SearchExtension
{
  public static Ai12SearchResult Search(V14GameState gameState, Ai12SearchResult alpha, Ai12SearchResult beta, Ai12TimeManagement timeManagement)
  {
    var lastMove = gameState.History[^1];
    if (lastMove.killed == null) return Ai12Evaluate.Evaluate(gameState);

    var legalMoves = gameState.getLegalMoves();
    var idleEvaluation = Ai12Evaluate.Evaluate(gameState);
    if (idleEvaluation.terminalLeaf) return idleEvaluation;

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
      var searchResult = Search(gameState, bestSearchResult, alpha, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(1)) break;

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