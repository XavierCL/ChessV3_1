public static class Ai12Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai12SearchResult Search(V14GameState gameState, int depth, int previousLegalCount, int previousSecondLegalCount, HashsetCache<V14BoardState, Ai12SearchResult> evaluationCache, Ai12TimeManagement timeManagement)
  {
    if (depth <= 1) return Ai12SearchExtension.Search(gameState, previousLegalCount, previousSecondLegalCount, evaluationCache, timeManagement);

    if (gameState.IsGameStateDraw()) return Ai12SearchResult.FromDraw(gameState.history.Count);

    var cacheEntry = evaluationCache.Get(gameState.boardState);
    if (cacheEntry != null && (cacheEntry.terminalLeaf || cacheEntry.depth >= depth && cacheEntry.depth >= 2)) return cacheEntry.ResetGameTurn(gameState.history.Count);

    var legalMoves = gameState.getLegalMovesWithoutGameStateCheck();
    var idleEvaluation = cacheEntry != null ? cacheEntry.ResetGameTurn(gameState.history.Count) : Ai12Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);

    if (idleEvaluation.terminalLeaf) return idleEvaluation;

    var bestSearchResult = idleEvaluation.SetValue(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue);
    var allTerminalLeaves = true;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, legalMoves.Count, previousLegalCount, evaluationCache, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.IsBetterThan(bestSearchResult, gameState))
      {
        // Return early if the best outcome can be achieved
        if (searchResult.IsBestTerminal(gameState))
        {
          var earlySearchResult = new Ai12SearchResult(
            idleEvaluation,
            searchResult,
            nodeCount,
            depth
          );

          evaluationCache.Set(gameState.boardState, earlySearchResult);

          return earlySearchResult;
        }

        bestSearchResult = searchResult;
      }
    }

    var finalSearchResult = new Ai12SearchResult(idleEvaluation, bestSearchResult, allTerminalLeaves, nodeCount, depth);
    evaluationCache.Set(gameState.boardState, finalSearchResult);
    return finalSearchResult;
  }
}