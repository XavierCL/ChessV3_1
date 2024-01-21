public static class Ai13Evaluate
{
  public static Ai13SearchResult Evaluate(V16GameState gameState)
  {
    return new Ai13SearchResult(
      gameState.boardState.bitBoards[V16BoardState.WhitePawn].bitCount() * 1
      + gameState.boardState.bitBoards[V16BoardState.BlackPawn].bitCount() * -1
      + gameState.boardState.bitBoards[V16BoardState.WhiteRook].bitCount() * 5
      + gameState.boardState.bitBoards[V16BoardState.BlackRook].bitCount() * -5
      + gameState.boardState.bitBoards[V16BoardState.WhiteKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V16BoardState.BlackKnight].bitCount() * -3
      + gameState.boardState.bitBoards[V16BoardState.WhiteBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V16BoardState.BlackBishop].bitCount() * -3
      + gameState.boardState.bitBoards[V16BoardState.WhiteQueen].bitCount() * 9
      + gameState.boardState.bitBoards[V16BoardState.BlackQueen].bitCount() * -9,
      false,
      1
    );
  }
}