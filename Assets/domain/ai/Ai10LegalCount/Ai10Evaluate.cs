public static class Ai10Evaluate
{
  public static Ai10SearchResult Evaluate(V14GameState gameState, int previousLegalCount, int previousSecondLegalCount)
  {
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return new Ai10SearchResult(double.MaxValue, true, 1, double.MaxValue, gameState.History.Count);
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return new Ai10SearchResult(double.MinValue, true, 1, double.MinValue, gameState.History.Count);
    }
    else if (endGameState == GameEndState.Draw)
    {
      return new Ai10SearchResult(0, true, 1, 0, gameState.History.Count);
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

    return new Ai10SearchResult(
      value + legalDiff,
      false,
      1,
      value,
      gameState.History.Count
    );
  }
}