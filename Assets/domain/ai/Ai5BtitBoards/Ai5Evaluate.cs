public static class Ai5Evaluate
{
  public static double Evaluate(V12GameState gameState)
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

    return gameState.boardState.bitBoards[V12BoardState.WhitePawn].bitCount() * 1
      + gameState.boardState.bitBoards[V12BoardState.BlackPawn].bitCount() * -1
      + gameState.boardState.bitBoards[V12BoardState.WhiteRook].bitCount() * 5
      + gameState.boardState.bitBoards[V12BoardState.BlackRook].bitCount() * -5
      + gameState.boardState.bitBoards[V12BoardState.WhiteKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V12BoardState.BlackKnight].bitCount() * -3
      + gameState.boardState.bitBoards[V12BoardState.WhiteBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V12BoardState.BlackBishop].bitCount() * -3
      + gameState.boardState.bitBoards[V12BoardState.WhiteQueen].bitCount() * 9
      + gameState.boardState.bitBoards[V12BoardState.BlackQueen].bitCount() * -9;
  }
}