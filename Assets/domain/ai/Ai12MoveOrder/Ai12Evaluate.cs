public static class Ai12Evaluate
{
  public static Ai12SearchResult Evaluate(V14GameState gameState, int previousLegalCount, int previousSecondLegalCount)
  {
    var endGameState = gameState.GetGameEndStateWithoutGameStateCheck();

    if (endGameState == GameEndState.WhiteWin)
    {
      return new Ai12SearchResult(double.MaxValue, true, 1, double.MaxValue, gameState.history.Count, 1);
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return new Ai12SearchResult(double.MinValue, true, 1, double.MinValue, gameState.history.Count, 1);
    }
    else if (endGameState == GameEndState.Draw)
    {
      return Ai12SearchResult.FromDraw(gameState.history.Count);
    }

    var value = gameState.boardState.bitBoards[V14BoardState.WhitePawn].bitCount() * 1
      + gameState.boardState.bitBoards[V14BoardState.BlackPawn].bitCount() * -1
      + gameState.boardState.bitBoards[V14BoardState.WhiteRook].bitCount() * 5
      + gameState.boardState.bitBoards[V14BoardState.BlackRook].bitCount() * -5
      + gameState.boardState.bitBoards[V14BoardState.WhiteKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V14BoardState.BlackKnight].bitCount() * -3
      + gameState.boardState.bitBoards[V14BoardState.WhiteBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V14BoardState.BlackBishop].bitCount() * -3
      + gameState.boardState.bitBoards[V14BoardState.WhiteQueen].bitCount() * 9
      + gameState.boardState.bitBoards[V14BoardState.BlackQueen].bitCount() * -9;

    var legalDiff = (gameState.boardState.whiteTurn ? -0.001 : 0.001) * (previousLegalCount - previousSecondLegalCount);

    return new Ai12SearchResult(
      value + legalDiff,
      false,
      1,
      value,
      gameState.history.Count,
      1
    );
  }
}