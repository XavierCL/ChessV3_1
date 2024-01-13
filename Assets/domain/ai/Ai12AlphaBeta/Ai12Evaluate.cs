public static class Ai12Evaluate
{
  public static Ai12SearchResult Evaluate(V14GameState gameState)
  {
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return new Ai12SearchResult(double.MaxValue, true, 1);
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return new Ai12SearchResult(double.MinValue, true, 1);
    }
    else if (endGameState == GameEndState.Draw)
    {
      return new Ai12SearchResult(0, true, 1);
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

    return new Ai12SearchResult(
      value,
      false,
      1
    );
  }
}