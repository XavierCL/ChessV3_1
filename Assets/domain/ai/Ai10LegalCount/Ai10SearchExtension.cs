public static class Ai10SearchExtension
{
  public static Ai10SearchResult Search(V14GameState gameState, int previousLegalCount, int previousSecondLegalCount, Ai10TimeManagement timeManagement)
  {
    var lastMove = gameState.History[^1];
    if (lastMove.killed == null) return Ai10Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);

    var legalMoves = gameState.getLegalMoves();
    var idleEvaluation = Ai10Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);
    if (idleEvaluation.terminalLeaf) return idleEvaluation;

    var allowedTarget = lastMove.killed.position.index;
    var bestSearchResult = idleEvaluation;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count, previousLegalCount, timeManagement);
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