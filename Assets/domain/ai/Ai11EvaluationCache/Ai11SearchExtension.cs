public static class Ai11SearchExtension
{
  public static Ai11SearchResult Search(V14GameState gameState, int previousLegalCount, int previousSecondLegalCount, MapCache<V14BoardState, Ai11SearchResult> evaluationCache, Ai11TimeManagement timeManagement)
  {
    if (gameState.IsGameStateDraw()) return Ai11SearchResult.FromDraw(gameState.History.Count);

    var cacheEntry = evaluationCache.Get(gameState.boardState);
    if (cacheEntry != null && (cacheEntry.terminalLeaf || cacheEntry.depth >= 2)) return cacheEntry.ResetGameTurn(gameState.History.Count);

    var lastMove = gameState.History[^1];
    if (lastMove.killed == null) return cacheEntry != null ? cacheEntry.ResetGameTurn(gameState.History.Count) : Ai11Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);

    var legalMoves = gameState.getLegalMovesWithoutGameStateCheck();
    var idleEvaluation = cacheEntry != null ? cacheEntry.ResetGameTurn(gameState.History.Count) : Ai11Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);
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
          var earlySearchResult = new Ai11SearchResult(
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

    var finalSearchResult = new Ai11SearchResult(idleEvaluation, bestSearchResult, false, nodeCount, 1);
    evaluationCache.Set(gameState.boardState, finalSearchResult);
    return finalSearchResult;
  }
}