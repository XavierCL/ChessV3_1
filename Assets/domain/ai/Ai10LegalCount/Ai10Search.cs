public static class Ai10Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai10SearchResult Search(V14GameState gameState, int depth, int previousLegalCount, int previousSecondLegalCount, Ai10TimeManagement timeManagement)
  {
    if (depth <= 1) return Ai10SearchExtension.Search(gameState, previousLegalCount, previousSecondLegalCount, timeManagement);

    var legalMoves = gameState.getLegalMoves();
    var idleEvaluation = Ai10Evaluate.Evaluate(gameState, previousLegalCount, previousSecondLegalCount);

    if (idleEvaluation.terminalLeaf) return idleEvaluation;

    var bestSearchResult = idleEvaluation.SetValue(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue);
    var allTerminalLeaves = true;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, legalMoves.Count, previousLegalCount, timeManagement);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (timeManagement.ShouldStop(depth)) break;

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

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

    return new Ai10SearchResult(idleEvaluation, bestSearchResult, allTerminalLeaves, nodeCount);
  }
}