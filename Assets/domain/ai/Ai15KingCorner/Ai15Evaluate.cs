using System;
using System.Collections.Generic;
using System.Linq;

public static class Ai15Evaluate
{
  public static Ai15SearchResult Evaluate(V17GameState gameState, Ai15SearchResult.HyperParameters hyperParameters)
  {
    var whiteScore = gameState.boardState.bitBoards[V17BoardState.WhitePawn].bitCount() * 1
      + gameState.boardState.bitBoards[V17BoardState.WhiteRook].bitCount() * 5
      + gameState.boardState.bitBoards[V17BoardState.WhiteKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V17BoardState.WhiteBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V17BoardState.WhiteQueen].bitCount() * 9;

    var blackScore = gameState.boardState.bitBoards[V17BoardState.BlackPawn].bitCount() * 1
      + gameState.boardState.bitBoards[V17BoardState.BlackRook].bitCount() * 5
      + gameState.boardState.bitBoards[V17BoardState.BlackKnight].bitCount() * 3
      + gameState.boardState.bitBoards[V17BoardState.BlackBishop].bitCount() * 3
      + gameState.boardState.bitBoards[V17BoardState.BlackQueen].bitCount() * 9;

    var middleKingEndGame = 0.0;

    if (whiteScore == 0)
    {
      var losingKingPosition = gameState.boardState.bitBoards[V17BoardState.WhiteKing].lsb();
      var losingKingCol = losingKingPosition.getCol();
      var losingKingRow = losingKingPosition.getRow();
      var midColDistance = losingKingCol - middleSquare;
      var midRowDistance = losingKingRow - middleSquare;
      middleKingEndGame -= hyperParameters.middleKingEndGame * (midColDistance * midColDistance + midRowDistance * midRowDistance);

      var winningKingPosition = gameState.boardState.bitBoards[V17BoardState.BlackKing].lsb();
      var kingColDistance = losingKingCol - winningKingPosition.getCol();
      var kingRowDistance = losingKingRow - winningKingPosition.getRow();
      middleKingEndGame += hyperParameters.middleKingEndGame * (kingColDistance * kingColDistance + kingRowDistance * kingRowDistance);
    }

    if (blackScore == 0)
    {
      var losingKingPosition = gameState.boardState.bitBoards[V17BoardState.BlackKing].lsb();
      var losingKingCol = losingKingPosition.getCol();
      var losingKingRow = losingKingPosition.getRow();
      var midColDistance = losingKingCol - middleSquare;
      var midRowDistance = losingKingRow - middleSquare;
      middleKingEndGame += hyperParameters.middleKingEndGame * (midColDistance * midColDistance + midRowDistance * midRowDistance);

      var winningKingPosition = gameState.boardState.bitBoards[V17BoardState.WhiteKing].lsb();
      var kingColDistance = losingKingCol - winningKingPosition.getCol();
      var kingRowDistance = losingKingRow - winningKingPosition.getRow();
      middleKingEndGame -= hyperParameters.middleKingEndGame * (kingColDistance * kingColDistance + kingRowDistance * kingRowDistance);
    }

    return new Ai15SearchResult(
      whiteScore - blackScore + middleKingEndGame,
      false,
      1
    );
  }

  public static IReadOnlyList<Move> SortMoves(IReadOnlyList<Move> legalMoves, V17GameState gameState)
  {
    var multiplier = gameState.boardState.whiteTurn ? -1 : 1;
    return legalMoves.OrderBy(move => multiplier * EvaluateMove(move, gameState)).ToArray();
  }

  public static double EvaluateMove(Move move, V17GameState gameState)
  {
    var moveValue = 0.0;

    if (move.promotion != PieceType.Nothing)
    {
      if (move.promotion == PieceType.WhiteQueen) moveValue += 8;
      else if (move.promotion == PieceType.BlackQueen) moveValue -= 8;
      else if (move.promotion == PieceType.WhiteKnight) moveValue += 2;
      else if (move.promotion == PieceType.BlackKnight) moveValue -= 2;
      else if (move.promotion == PieceType.WhiteRook) moveValue += 4;
      else if (move.promotion == PieceType.BlackRook) moveValue -= 4;
      else if (move.promotion == PieceType.WhiteBishop) moveValue += 2;
      else if (move.promotion == PieceType.BlackBishop) moveValue -= 2;
    }

    var targetBitBoard = move.target.index.toBitBoard();
    if ((targetBitBoard & gameState.boardState.allPiecesBitBoard) != 0)
    {
      if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.WhitePawn]) != 0) moveValue -= 1;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.BlackPawn]) != 0) moveValue += 1;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.WhiteRook]) != 0) moveValue -= 5;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.BlackRook]) != 0) moveValue += 5;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.WhiteKnight]) != 0) moveValue -= 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.BlackKnight]) != 0) moveValue += 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.WhiteBishop]) != 0) moveValue -= 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.BlackBishop]) != 0) moveValue += 3;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.WhiteQueen]) != 0) moveValue -= 9;
      else if ((targetBitBoard & gameState.boardState.bitBoards[V17BoardState.BlackQueen]) != 0) moveValue += 9;
    }

    return moveValue;
  }

  private static readonly double middleSquare = 3.5;
}