using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class V9LegalMoveGenerator
{
  /// <summary>
  /// Doesn't take end game draws in consideration (three-fold nor 50 moves)
  /// </summary>
  public static List<Move> GenerateLegalMoves(this V9GameState gameState)
  {
    if (gameState.StaleTurns >= 100) return new List<Move>();
    if (gameState.snapshots.GetValueOrDefault(gameState.boardState) >= 2) return new List<Move>();
    var pseudoLegalMoves = GeneratePseudoLegalMoves(gameState.boardState, gameState.BoardState.WhiteTurn);
    var legalMoves = new List<Move>(pseudoLegalMoves.Count);

    for (var index = 0; index < pseudoLegalMoves.Count; ++index)
    {
      var move = pseudoLegalMoves[index];
      if (!CanKingDieAfterMove(gameState.boardState, move, gameState.BoardState.WhiteTurn))
      {
        legalMoves.Add(move);
      }
    }

    return legalMoves;
  }

  public static bool CanOwnKingDie(V9GameState gameState)
  {
    return CanKingDie(gameState.boardState, gameState.BoardState.WhiteTurn);
  }

  private static bool CanKingDieAfterMove(V9BoardState boardState, Move ownMove, bool whiteKing)
  {
    var kingMightBeInCheckState = boardState.PlayMove(ownMove).boardState;
    return CanKingDie(kingMightBeInCheckState, whiteKing);
  }

  private static bool CanKingDie(V9BoardState boardState, bool whiteKing)
  {
    var kingPosition = whiteKing ? boardState.whiteKingPosition : boardState.blackKingPosition;
    var kingCol = kingPosition.getCol();
    var kingRow = kingPosition.getRow();
    for (var rowIncrement = -1; rowIncrement < 2; ++rowIncrement)
    {
      for (var colIncrement = -1; colIncrement < 2; ++colIncrement)
      {
        if (rowIncrement == 0 && colIncrement == 0) continue;

        var firstPiecePosition = GetFirstPiecePositionAtRay(colIncrement, rowIncrement, boardState.allBitBoard, kingPosition);

        if (firstPiecePosition == -1) continue;

        if (firstPiecePosition < 0 || firstPiecePosition > 63)
        {
          Debug.Log("uh ho");
        }

        var pieceAtRay = boardState.boardPieces[firstPiecePosition];

        if (pieceAtRay.IsWhite() == whiteKing) continue;
        if (pieceAtRay.IsQueen()) return true;

        var pieceAtRayRow = firstPiecePosition.getRow();
        var distance = Math.Max(Math.Abs(firstPiecePosition.getCol() - kingCol), Math.Abs(pieceAtRayRow - kingRow));

        if (distance <= 1 && pieceAtRay.IsKing()) return true;

        var isLineRay = (rowIncrement + colIncrement + 2) % 2 == 1;

        if (isLineRay)
        {
          if (pieceAtRay.IsRook()) return true;
        }
        else
        {
          if (pieceAtRay.IsBishop()) return true;
          if (distance > 1) continue;
          if (pieceAtRay.IsPawn() && (whiteKing ? (pieceAtRayRow == kingRow + 1) : (pieceAtRayRow == kingRow - 1))) return true;
        }
      }
    }

    var jumps = V9Precomputed.knightJumps[kingPosition];

    for (var index = 0; index < jumps.Length; ++index)
    {
      var jump = jumps[index];
      var targetPieceType = boardState.boardPieces[jump];
      if (targetPieceType.IsKnight() && targetPieceType.IsWhite() != whiteKing) return true;
    }

    return false;
  }

  private static List<Move> GeneratePseudoLegalMoves(V9BoardState boardState, bool white)
  {
    var pseudoLegalMoves = new List<Move>();

    for (var index = 0; index < boardState.pieceIndices.Length; ++index)
    {
      var position = boardState.pieceIndices[index];
      var pieceType = boardState.GetPieceTypeAtPosition(position);
      if (pieceType.IsWhite() == white)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalMoves(boardState, position.toBoardPosition(), pieceType));
      }
    }

    return pseudoLegalMoves;
  }

  private static List<Move> GetPseudoLegalMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    switch (pieceType)
    {
      case PieceType.WhitePawn:
      case PieceType.BlackPawn:
        return GetPseudoLegalPawnMoves(boardState, position, pieceType);
      case PieceType.WhiteRook:
      case PieceType.BlackRook:
        return GetPseudoLegalRookMoves(boardState, position, pieceType);
      case PieceType.WhiteKnight:
      case PieceType.BlackKnight:
        return GetPseudoLegalKnightMoves(boardState, position, pieceType);
      case PieceType.WhiteBishop:
      case PieceType.BlackBishop:
        return GetPseudoLegalBishopMoves(boardState, position, pieceType);
      case PieceType.WhiteQueen:
      case PieceType.BlackQueen:
        return GetPseudoLegalQueenMoves(boardState, position, pieceType);
      case PieceType.WhiteKing:
      case PieceType.BlackKing:
        return GetPseudoLegalKingMoves(boardState, position, pieceType);
      default:
        throw new System.Exception("Not a valid piece");
    }
  }

  private static List<Move> GetPseudoLegalPawnMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    var ownPawnStartingY = pieceType.IsWhite() ? 1 : 6;
    var increment = pieceType.IsWhite() ? 1 : -1;
    var stopCondition = pieceType.IsWhite() ? 7 : 0;

    var moves = new List<Move>(4);

    var oneUpPosition = BoardPosition.fromColRow(position.col, position.row + increment);
    var oneUpPieceExists = boardState.GetPieceTypeAtPosition(oneUpPosition) != PieceType.Nothing;
    if (!oneUpPieceExists)
    {
      moves.Add(new Move(position, oneUpPosition.toBoardPosition(), PieceType.Nothing));
    }

    if (position.row == ownPawnStartingY)
    {
      var twoUpPosition = BoardPosition.fromColRow(position.col, position.row + increment * 2);
      if (!oneUpPieceExists && boardState.GetPieceTypeAtPosition(twoUpPosition) == PieceType.Nothing)
      {
        moves.Add(new Move(position, twoUpPosition.toBoardPosition(), PieceType.Nothing));
      }
    }

    if (position.col != 0)
    {
      var captureLeftPosition = BoardPosition.fromColRow(position.col - 1, position.row + increment);
      var captureLeft = boardState.GetPieceTypeAtPosition(captureLeftPosition);
      if (captureLeft != PieceType.Nothing && captureLeft.IsWhite() != pieceType.IsWhite())
      {
        moves.Add(new Move(position, captureLeftPosition.toBoardPosition(), PieceType.Nothing));
      }
    }

    if (position.col != 7)
    {
      var captureRightPosition = BoardPosition.fromColRow(position.col + 1, position.row + increment);
      var captureRight = boardState.GetPieceTypeAtPosition(captureRightPosition);
      if (captureRight != PieceType.Nothing && captureRight.IsWhite() != pieceType.IsWhite())
      {
        moves.Add(new Move(position, captureRightPosition.toBoardPosition(), PieceType.Nothing));
      }
    }

    // En passant
    var enemyFourthRow = pieceType.IsWhite() ? 4 : 3;
    if (position.row == enemyFourthRow)
    {
      if (position.col != 0)
      {
        var captureLeftPosition = new BoardPosition(position.col - 1, position.row + increment);
        var neighbourLeftPosition = BoardPosition.fromColRow(position.col - 1, position.row);
        var neighbourLeft = boardState.GetPieceTypeAtPosition(neighbourLeftPosition);
        if (boardState.enPassantColumn == neighbourLeftPosition.getCol() && neighbourLeft.IsWhite() != pieceType.IsWhite() && neighbourLeft.IsPawn())
        {
          moves.Add(new Move(position, captureLeftPosition, PieceType.Nothing));
        }
      }

      if (position.col != 7)
      {
        var captureRightPosition = new BoardPosition(position.col + 1, position.row + increment);
        var neighbourRightPosition = BoardPosition.fromColRow(position.col + 1, position.row);
        var neighbourRight = boardState.GetPieceTypeAtPosition(neighbourRightPosition);
        if (boardState.enPassantColumn == neighbourRightPosition.getCol() && neighbourRight.IsWhite() != pieceType.IsWhite() && neighbourRight.IsPawn())
        {
          moves.Add(new Move(position, captureRightPosition, PieceType.Nothing));
        }
      }
    }

    if (position.row + increment != stopCondition)
    {
      return moves;
    }

    var promotionMoves = new List<Move>(moves.Count * 4);
    var promotions = pieceType.IsWhite()
        ? new PieceType[] { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
        : new PieceType[] { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

    // Multiply moves by promotions
    for (var moveIndex = 0; moveIndex < moves.Count; ++moveIndex)
    {
      for (var promotionIndex = 0; promotionIndex < promotions.Length; ++promotionIndex)
      {
        promotionMoves.Add(new Move(position, moves[moveIndex].target, promotions[promotionIndex]));
      }
    }

    return promotionMoves;
  }

  private static List<Move> GetPseudoLegalRookMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    return GetPseudoRayMoves(boardState, position, 1, 0, pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(boardState, position, -1, 0, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 0, 1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 0, -1, pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalKnightMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    var validMoves = new List<Move>(8);
    var jumps = V9Precomputed.knightJumps[position.index];

    for (var jumpIndex = 0; jumpIndex < jumps.Length; ++jumpIndex)
    {
      var jump = jumps[jumpIndex];
      var collision = boardState.GetPieceTypeAtPosition(jump);
      if (collision != PieceType.Nothing && collision.IsWhite() == pieceType.IsWhite()) continue;
      validMoves.Add(new Move(position, jump.toBoardPosition(), PieceType.Nothing));
    }

    return validMoves;
  }

  private static List<Move> GetPseudoLegalBishopMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    return GetPseudoRayMoves(boardState, position, 1, 1, pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(boardState, position, -1, 1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, -1, -1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 1, -1, pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalQueenMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    return GetPseudoRayMoves(boardState, position, 1, 0, pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(boardState, position, -1, 0, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 0, 1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 0, -1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 1, 1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, -1, 1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, -1, -1, pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, position, 1, -1, pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalKingMoves(V9BoardState boardState, BoardPosition position, PieceType pieceType)
  {
    var landings = new[]{
      new { col = position.col - 1, row = position.row - 1},
      new { col = position.col - 1, row = position.row    },
      new { col = position.col - 1, row = position.row + 1},
      new { col = position.col    , row = position.row + 1},
      new { col = position.col + 1, row = position.row + 1},
      new { col = position.col + 1, row = position.row    },
      new { col = position.col + 1, row = position.row - 1},
      new { col = position.col    , row = position.row - 1},
    };

    var kingMoves = new List<Move>(8);

    for (var landingIndex = 0; landingIndex < landings.Length; ++landingIndex)
    {
      var landing = landings[landingIndex];
      if (!BoardPosition.IsInBoard(landing.col, landing.row)) continue;
      var collision = boardState.GetPieceTypeAtPosition(BoardPosition.fromColRow(landing.col, landing.row));
      if (collision != PieceType.Nothing && collision.IsWhite() == pieceType.IsWhite()) continue;

      kingMoves.Add(new Move(position, new BoardPosition(landing.col, landing.row), PieceType.Nothing));
    }

    var castles = new[] {
      new { castle = CastleFlags.WhiteKing, emptyPositions = new List<int>{ BoardPosition.fromColRow(5, 0), BoardPosition.fromColRow(6, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 0), new BoardPosition(6, 0) } },
      new { castle = CastleFlags.WhiteQueen, emptyPositions = new List<int>{ BoardPosition.fromColRow(3, 0), BoardPosition.fromColRow(2, 0), BoardPosition.fromColRow(1, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 0), new BoardPosition(2, 0) } },
      new { castle = CastleFlags.BlackKing, emptyPositions = new List<int>{ BoardPosition.fromColRow(5, 7), BoardPosition.fromColRow(6, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 7), new BoardPosition(6, 7) } },
      new { castle = CastleFlags.BlackQueen, emptyPositions = new List<int>{ BoardPosition.fromColRow(3, 7), BoardPosition.fromColRow(2, 7), BoardPosition.fromColRow(1, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 7), new BoardPosition(2, 7) } },
    };

    for (var castleIndex = 0; castleIndex < castles.Length; ++castleIndex)
    {
      var castle = castles[castleIndex];
      if (!boardState.castleFlags.HasFlag(castle.castle)) continue;
      if (pieceType.IsWhite() != castle.castle.IsWhite()) continue;

      bool positionsAreEmpty = true;
      foreach (var emptyPosition in castle.emptyPositions)
      {
        if (boardState.GetPieceTypeAtPosition(emptyPosition) != PieceType.Nothing)
        {
          positionsAreEmpty = false;
          break;
        }
      }

      if (!positionsAreEmpty) continue;

      var lastCheckedBoardState = boardState;
      var lastKingPosition = position;

      if (CanKingDie(lastCheckedBoardState, pieceType.IsWhite())) continue;

      bool kingIsNeverInCheck = true;
      foreach (var noCheckPosition in castle.noCheckPositions)
      {
        lastCheckedBoardState = lastCheckedBoardState.PlayMove(new Move(lastKingPosition, noCheckPosition, PieceType.Nothing)).boardState;
        lastKingPosition = noCheckPosition;
        if (CanKingDie(lastCheckedBoardState, pieceType.IsWhite()))
        {
          kingIsNeverInCheck = false;
          break;
        }
      }

      if (!kingIsNeverInCheck) continue;

      kingMoves.Add(new Move(position, castle.noCheckPositions[^1], PieceType.Nothing));
    }

    return kingMoves;
  }

  private static List<Move> GetPseudoRayMoves(V9BoardState boardState, BoardPosition position, int colIncrement, int rowIncrement, bool isWhite)
  {
    var moves = new List<Move>();
    var col = position.col + colIncrement;
    var row = position.row + rowIncrement;
    while (col <= 7 && col >= 0 && row <= 7 && row >= 0)
    {
      var currentPosition = BoardPosition.fromColRow(col, row);
      var foundCollision = boardState.GetPieceTypeAtPosition(currentPosition);

      if (foundCollision != PieceType.Nothing && foundCollision.IsWhite() == isWhite) return moves;

      moves.Add(new Move(position, currentPosition.toBoardPosition(), PieceType.Nothing));

      if (foundCollision != PieceType.Nothing) return moves;

      col += colIncrement;
      row += rowIncrement;
    }

    return moves;
  }

  public static int GetFirstPiecePositionAtRay(int colIncrement, int rowIncrement, ulong allPieces, int position)
  {
    var values = V9Precomputed.bitBoardAtRayBranches[(colIncrement + 1) * 3 + rowIncrement + 1][position] & allPieces;
    if (rowIncrement < 0 || rowIncrement == 0 && colIncrement < 0) return values.msb();
    return values.lsb();
  }
}