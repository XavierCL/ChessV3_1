public static class Ai9SearchExtension
{
  public static Ai9SearchResult Search(V14GameState gameState, Ai9TimeManagement timeManagement)
  {
    var lastMove = gameState.history[^1];
    if (lastMove.killed == null) return Ai9Evaluate.Evaluate(gameState);

    var legalMoves = gameState.GenerateLegalMoves();
    var idleEvaluation = Ai9Evaluate.Evaluate(gameState);
    if (idleEvaluation.terminalLeaf) return idleEvaluation;

    var allowedTarget = lastMove.killed.position.index;
    var bestSearchResult = idleEvaluation;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(1)) break;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          return new Ai9SearchResult(
            idleEvaluation,
            searchResult,
            nodeCount
          );
        }

        bestSearchResult = searchResult;
      }
    }

    return new Ai9SearchResult(idleEvaluation, bestSearchResult, nodeCount);
  }
}