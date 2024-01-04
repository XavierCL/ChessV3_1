public static class Ai8Evaluate
{
  public static Ai8SearchResult Evaluate(V14GameState gameState)
  {
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return new Ai8SearchResult(double.MaxValue, true, 1);
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return new Ai8SearchResult(double.MinValue, true, 1);
    }
    else if (endGameState == GameEndState.Draw)
    {
      return new Ai8SearchResult(0, true, 1);
    }

    return new Ai8SearchResult(
      gameState.boardState.bitBoards[V14BoardState.WhitePawn].bitCount() * 1
      + gameState.boardState.bitBoards[V14BoardState.BlackPawn].bitCount() * -1
      + gameState.boardState.bitBoards[V14BoardState.WhiteRook].bitCount() * 5
      + gameState.boardState.bitBoards[V14BoardState.BlackRook].bitCount() * -5
      + gameState.boardState.bitBoards[V14BoardState.WhiteKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V14BoardState.BlackKnight].bitCount() * -3
      + gameState.boardState.bitBoards[V14BoardState.WhiteBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V14BoardState.BlackBishop].bitCount() * -3
      + gameState.boardState.bitBoards[V14BoardState.WhiteQueen].bitCount() * 9
      + gameState.boardState.bitBoards[V14BoardState.BlackQueen].bitCount() * -9,
      false,
      1
    );
  }
}