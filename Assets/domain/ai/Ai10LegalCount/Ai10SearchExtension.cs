public static class Ai10SearchExtension
{
  public static Ai10SearchResult Search(V14GameState gameState, Ai10SearchResult idleEvaluation, Ai10TimeManagement timeManagement)
  {
    if (idleEvaluation.terminalLeaf) return idleEvaluation;

    var legalMoves = gameState.GenerateLegalMoves();
    var lastMove = gameState.history[^1];

    if (lastMove.killed == null) return idleEvaluation;

    var allowedTarget = lastMove.killed.position.index;
    var bestSearchResult = idleEvaluation;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var nextIdle = Ai10Evaluate.Evaluate(gameState, legalMoves.Count);
      var searchResult = Search(gameState, nextIdle, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(1)) break;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          return new Ai10SearchResult(
            idleEvaluation,
            searchResult,
            nodeCount
          );
        }

        bestSearchResult = searchResult;
      }
    }

    return new Ai10SearchResult(idleEvaluation, bestSearchResult, nodeCount);
  }
}