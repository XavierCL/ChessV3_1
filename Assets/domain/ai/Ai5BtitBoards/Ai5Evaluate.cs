public static class Ai5Evaluate
{
  public static double Evaluate(V14GameState gameState)
  {
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return double.MaxValue;
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return double.MinValue;
    }
    else if (endGameState == GameEndState.Draw)
    {
      return 0;
    }

    return gameState.boardState.bitBoards[V14BoardState.WhitePawn].bitCount() * 1
      + gameState.boardState.bitBoards[V14BoardState.BlackPawn].bitCount() * -1
      + gameState.boardState.bitBoards[V14BoardState.WhiteRook].bitCount() * 5
      + gameState.boardState.bitBoards[V14BoardState.BlackRook].bitCount() * -5
      + gameState.boardState.bitBoards[V14BoardState.WhiteKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V14BoardState.BlackKnight].bitCount() * -3
      + gameState.boardState.bitBoards[V14BoardState.WhiteBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V14BoardState.BlackBishop].bitCount() * -3
      + gameState.boardState.bitBoards[V14BoardState.WhiteQueen].bitCount() * 9
      + gameState.boardState.bitBoards[V14BoardState.BlackQueen].bitCount() * -9;
  }
}