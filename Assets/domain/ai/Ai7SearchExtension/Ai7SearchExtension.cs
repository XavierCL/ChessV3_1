public static class Ai7SearchExtension
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static Ai7SearchResult Search(V14GameState gameState)
  {
    var idleEvaluation = Ai7Evaluate.Evaluate(gameState);

    if (idleEvaluation.terminalLeaf) return idleEvaluation;
    var lastMove = gameState.history[^1];

    if (lastMove.killed == null) return idleEvaluation;

    var allowedTarget = lastMove.killed.position.index;
    var legalMoves = gameState.getLegalMoves();
    var bestValue = idleEvaluation.value;
    var nodeCount = 1L;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      var legalMove = legalMoves[legalMoveIndex];
      if (legalMove.target.index != allowedTarget) continue;

      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState);
      nodeCount += searchResult.nodeCount;
      gameState.UndoMove();

      if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
      {
        bestValue = searchResult.value;
      }

      // Return early if the best outcome can be achieved
      if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
      {
        return new Ai7SearchResult(bestValue, true, nodeCount);
      }
    }

    return new Ai7SearchResult(bestValue, false, nodeCount);
  }
}