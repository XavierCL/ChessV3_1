public static class Ai12SearchExtension
{
  public static Ai12SearchResult Search(V14GameState gameState, int previousLegalCount, int previousSecondLegalCount, HashsetCache<V14BoardState, Ai12SearchResult> evaluationCache, Ai12TimeManagement timeManagement)
  {
    if (gameState.IsGameStateDraw()) return Ai12SearchResult.FromDraw(gameState.history.Count);

    var cacheEntry = evaluationCache.Get(gameState.boardState);
    if (cacheEntry != null && (cacheEntry.terminalLeaf || cacheEntry.depth >= 2)) return cacheEntry.ResetGameTurn(gameState.history.Count);

    var lastMove = gameState.history[^1];
    if (lastMove.killed == null) return cacheEntry != null ? cacheEntry.ResetGameTurn(gameState.history.Count) : Ai12Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);

    var legalMoves = gameState.getLegalMovesWithoutGameStateCheck();
    var idleEvaluation = cacheEntry != null ? cacheEntry.ResetGameTurn(gameState.history.Count) : Ai12Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);
    if (idleEvaluation.terminalLeaf) return idleEvaluation;

    var allowedTarget = lastMove.killed.position.index;
    var bestSearchResult = idleEvaluation;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count, previousLegalCount, evaluationCache, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(1)) break;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          var earlySearchResult = new Ai12SearchResult(
            idleEvaluation,
            searchResult,
            nodeCount,
            1
          );

          evaluationCache.Set(gameState.boardState, earlySearchResult);

          return earlySearchResult;
        }

        bestSearchResult = searchResult;
      }
    }

    var finalSearchResult = new Ai12SearchResult(idleEvaluation, bestSearchResult, false, nodeCount, 1);
    evaluationCache.Set(gameState.boardState, finalSearchResult);
    return finalSearchResult;
  }
}