public static class Ai9Evaluate
{
  public static Ai9SearchResult Evaluate(V14GameState gameState)
  {
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return new Ai9SearchResult(double.MaxValue, true, 1, double.MaxValue, gameState.history.Count);
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return new Ai9SearchResult(double.MinValue, true, 1, double.MinValue, gameState.history.Count);
    }
    else if (endGameState == GameEndState.Draw)
    {
      return new Ai9SearchResult(0, true, 1, 0, gameState.history.Count);
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

    return new Ai9SearchResult(
      value,
      false,
      1,
      value,
      gameState.history.Count
    );
  }
}