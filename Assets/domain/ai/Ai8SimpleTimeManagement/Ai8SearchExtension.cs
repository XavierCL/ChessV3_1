public static class Ai8SearchExtension
{
  public static Ai8SearchResult Search(V14GameState gameState, Ai8TimeManagement timeManagement)
  {
    var idleEvaluation = Ai8Evaluate.Evaluate(gameState);

    if (idleEvaluation.terminalLeaf) return idleEvaluation;
    var lastMove = gameState.history[^1];

    if (lastMove.killed == null) return idleEvaluation;

    var allowedTarget = lastMove.killed.position.index;
    var legalMoves = gameState.GenerateLegalMoves();
    var bestValue = idleEvaluation.value;
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

      if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
      {
        bestValue = searchResult.value;
      }

      // Return early if the best outcome can be achieved
      if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
      {
        return new Ai8SearchResult(bestValue, true, nodeCount);
      }
    }

    return new Ai8SearchResult(bestValue, false, nodeCount);
  }
}