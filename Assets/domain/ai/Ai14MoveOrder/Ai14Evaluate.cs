using System.Collections.Generic;
using System.Linq;

public static class Ai14Evaluate
{
  public static Ai14SearchResult Evaluate(V16GameState gameState)
  {
    return new Ai14SearchResult(
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

  public static IReadOnlyList<Move> SortMoves(IReadOnlyList<Move> legalMoves, V16GameState gameState)
  {
    var allPieces = 0ul;

    for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
    {
      allPieces |= gameState.boardState.bitBoards[bitBoardIndex];
    }

    var multiplier = gameState.boardState.whiteTurn ? 1 : -1;
    return legalMoves.OrderBy(move => multiplier * EvaluateMove(move, gameState, allPieces)).ToArray();
  }

  public static double EvaluateMove(Move move, V16GameState gameState, ulong allPieces)
  {
    var moveValue = 0.0;

    if (move.promotion != PieceType.Nothing)
    {
      if (move.promotion == PieceType.WhiteRook) moveValue += 4;
      else if (move.promotion == PieceType.WhiteKnight) moveValue += 2;
      else if (move.promotion == PieceType.WhiteBishop) moveValue += 2;
      else if (move.promotion == PieceType.WhiteQueen) moveValue += 8;
      else if (move.promotion == PieceType.BlackRook) moveValue -= 4;
      else if (move.promotion == PieceType.BlackKnight) moveValue -= 2;
      else if (move.promotion == PieceType.BlackBishop) moveValue -= 2;
      else if (move.promotion == PieceType.BlackQueen) moveValue -= 8;
    }

    var targetBitBoard = move.target.index.toBitBoard();
    if ((targetBitBoard & allPieces) != 0)
    {
      if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.WhitePawn]) != 0) moveValue -= 1;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.BlackPawn]) != 0) moveValue += 1;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.WhiteRook]) != 0) moveValue -= 5;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.BlackRook]) != 0) moveValue += 5;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.WhiteKnight]) != 0) moveValue -= 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.BlackKnight]) != 0) moveValue += 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.WhiteBishop]) != 0) moveValue -= 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.BlackBishop]) != 0) moveValue += 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.WhiteQueen]) != 0) moveValue -= 9;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V16BoardState.BlackQueen]) != 0) moveValue += 9;
    }

    return moveValue;
  }
}