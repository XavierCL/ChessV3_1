public static class Ai5Search
{
  // Depths are decreasing. A depth of 1 means evaluation
  public static SearchResult Search(V12GameState gameState, int depth, Ai5TimeManagement timeManagement)
  {
    var legalMoves = gameState.getLegalMoves();
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return new SearchResult(double.MaxValue, true);
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return new SearchResult(double.MinValue, true);
    }
    else if (endGameState == GameEndState.Draw)
    {
      return new SearchResult(0, true);
    }

    if (depth <= 1 && legalMoves.Count != 1) return new SearchResult(Ai5Evaluate.Evaluate(gameState), false);

    var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
    var allTerminalLeaves = true;

    for (var legalMoveIndex = 0; legalMoveIndex < legalMoves.Count; ++legalMoveIndex)
    {
      if (timeManagement.ShouldStop()) return new SearchResult(bestValue, false);
      gameState.PlayMove(legalMoves[legalMoveIndex]);
      var searchResult = Search(gameState, legalMoves.Count == 1 ? depth : depth - 1, timeManagement);
      gameState.UndoMove();

      allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

      if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
      {
        bestValue = searchResult.value;
      }

      // Return early if the best outcome can be achieved
      if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
      {
        return new SearchResult(bestValue, true);
      }
    }

    return new SearchResult(bestValue, allTerminalLeaves);
  }

  public class SearchResult
  {
    public readonly double value;
    public readonly bool terminalLeaf;

    public SearchResult(double value, bool terminalLeaf)
    {
      this.value = value;
      this.terminalLeaf = terminalLeaf;
    }
  }
}