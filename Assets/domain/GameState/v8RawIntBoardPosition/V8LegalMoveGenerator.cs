using System;
using System.Collections.Generic;
using System.Linq;

public static class V8LegalMoveGenerator
{
  /// <summary>
  /// Doesn't take end game draws in consideration (three-fold nor 50 moves)
  /// </summary>
  public static List<Move> GenerateLegalMoves(this V8GameState gameState)
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

  public static bool CanOwnKingDie(V8GameState gameState)
  {
    return CanKingDie(gameState.boardState, gameState.BoardState.WhiteTurn);
  }

  private static bool CanKingDieAfterMove(V8BoardState boardState, Move ownMove, bool whiteKing)
  {
    var kingMightBeInCheckState = boardState.PlayMove(ownMove).boardState;
    return CanKingDie(kingMightBeInCheckState, whiteKing);
  }

  private static bool CanKingDie(V8BoardState boardState, bool whiteKing)
  {
    var kingPosition = whiteKing ? boardState.whiteKingPosition : boardState.blackKingPosition;
    var kingCol = kingPosition.col;
    var kingRow = kingPosition.row;
    for (var rowIncrement = -1; rowIncrement < 2; ++rowIncrement)
    {
      for (var colIncrement = -1; colIncrement < 2; ++colIncrement)
      {
        if (rowIncrement == 0 && colIncrement == 0) continue;

        var pieceAtRay = GetRayNextPiece(boardState, kingPosition, colIncrement, rowIncrement);
        if (pieceAtRay == null || pieceAtRay.pieceType.IsWhite() == whiteKing) continue;
        var pieceAtRayRow = pieceAtRay.position.getRow();

        if (pieceAtRay.pieceType.IsQueen()) return true;

        var distance = Math.Max(Math.Abs(pieceAtRay.position.getCol() - kingCol), Math.Abs(pieceAtRayRow - kingRow));

        if (distance <= 1 && pieceAtRay.pieceType.IsKing()) return true;

        var isLineRay = (rowIncrement + colIncrement + 2) % 2 == 1;

        if (isLineRay)
        {
          if (pieceAtRay.pieceType.IsRook()) return true;
        }
        else
        {
          if (pieceAtRay.pieceType.IsBishop()) return true;
          if (distance > 1) continue;
          if (pieceAtRay.pieceType.IsPawn() && (whiteKing ? (pieceAtRayRow == kingRow + 1) : (pieceAtRayRow == kingRow - 1))) return true;
        }
      }
    }

    var jumps = V8Precomputed.knightJumps[kingPosition.index];

    for (var index = 0; index < jumps.Length; ++index)
    {
      var jump = jumps[index];
      var targetPieceType = boardState.boardPieces[jump];
      if (targetPieceType.IsWhite() != whiteKing && targetPieceType.IsKnight()) return true;
    }

    return false;
  }

  private static List<Move> GeneratePseudoLegalMoves(V8BoardState boardState, bool white)
  {
    var pseudoLegalMoves = new List<Move>();

    for (var index = 0; index < boardState.piecePositions.Count; ++index)
    {
      var piecePosition = boardState.piecePositions[index];
      if (piecePosition.pieceType.IsWhite() == white)
      {
        pseudoLegalMoves.AddRange(GetPseudoLegalMoves(boardState, piecePosition));
      }
    }

    return pseudoLegalMoves;
  }

  private static List<Move> GetPseudoLegalMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    switch (piecePosition.pieceType)
    {
      case PieceType.WhitePawn:
      case PieceType.BlackPawn:
        return GetPseudoLegalPawnMoves(boardState, piecePosition);
      case PieceType.WhiteRook:
      case PieceType.BlackRook:
        return GetPseudoLegalRookMoves(boardState, piecePosition);
      case PieceType.WhiteKnight:
      case PieceType.BlackKnight:
        return GetPseudoLegalKnightMoves(boardState, piecePosition);
      case PieceType.WhiteBishop:
      case PieceType.BlackBishop:
        return GetPseudoLegalBishopMoves(boardState, piecePosition);
      case PieceType.WhiteQueen:
      case PieceType.BlackQueen:
        return GetPseudoLegalQueenMoves(boardState, piecePosition);
      case PieceType.WhiteKing:
      case PieceType.BlackKing:
        return GetPseudoLegalKingMoves(boardState, piecePosition);
      default:
        throw new System.Exception("Not a valid piece");
    }
  }

  private static List<Move> GetPseudoLegalPawnMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    var ownPawnStartingY = piecePosition.pieceType.IsWhite() ? 1 : 6;
    var increment = piecePosition.pieceType.IsWhite() ? 1 : -1;
    var stopCondition = piecePosition.pieceType.IsWhite() ? 7 : 0;

    var moves = new List<Move>(4);

    var oneUpPosition = BoardPosition.fromColRow(piecePosition.position.col, piecePosition.position.row + increment);
    var oneUpPieceExists = boardState.GetPieceTypeAtPosition(oneUpPosition) != PieceType.Nothing;
    if (!oneUpPieceExists)
    {
      moves.Add(new Move(piecePosition.position, oneUpPosition.toBoardPosition(), PieceType.Nothing));
    }

    if (piecePosition.position.row == ownPawnStartingY)
    {
      var twoUpPosition = BoardPosition.fromColRow(piecePosition.position.col, piecePosition.position.row + increment * 2);
      if (!oneUpPieceExists && boardState.GetPieceTypeAtPosition(twoUpPosition) == PieceType.Nothing)
      {
        moves.Add(new Move(piecePosition.position, twoUpPosition.toBoardPosition(), PieceType.Nothing));
      }
    }

    if (piecePosition.position.col != 0)
    {
      var captureLeftPosition = BoardPosition.fromColRow(piecePosition.position.col - 1, piecePosition.position.row + increment);
      var captureLeft = boardState.GetPieceTypeAtPosition(captureLeftPosition);
      if (captureLeft != PieceType.Nothing && captureLeft.IsWhite() != piecePosition.pieceType.IsWhite())
      {
        moves.Add(new Move(piecePosition.position, captureLeftPosition.toBoardPosition(), PieceType.Nothing));
      }
    }

    if (piecePosition.position.col != 7)
    {
      var captureRightPosition = BoardPosition.fromColRow(piecePosition.position.col + 1, piecePosition.position.row + increment);
      var captureRight = boardState.GetPieceTypeAtPosition(captureRightPosition);
      if (captureRight != PieceType.Nothing && captureRight.IsWhite() != piecePosition.pieceType.IsWhite())
      {
        moves.Add(new Move(piecePosition.position, captureRightPosition.toBoardPosition(), PieceType.Nothing));
      }
    }

    // En passant
    var enemyFourthRow = piecePosition.pieceType.IsWhite() ? 4 : 3;
    if (piecePosition.position.row == enemyFourthRow)
    {
      if (piecePosition.position.col != 0)
      {
        var captureLeftPosition = new BoardPosition(piecePosition.position.col - 1, piecePosition.position.row + increment);
        var neighbourLeftPosition = BoardPosition.fromColRow(piecePosition.position.col - 1, piecePosition.position.row);
        var neighbourLeft = boardState.GetPieceTypeAtPosition(neighbourLeftPosition);
        if (boardState.EnPassantColumn == neighbourLeftPosition.getCol() && neighbourLeft.IsWhite() != piecePosition.pieceType.IsWhite() && neighbourLeft.IsPawn())
        {
          moves.Add(new Move(piecePosition.position, captureLeftPosition, PieceType.Nothing));
        }
      }

      if (piecePosition.position.col != 7)
      {
        var captureRightPosition = new BoardPosition(piecePosition.position.col + 1, piecePosition.position.row + increment);
        var neighbourRightPosition = BoardPosition.fromColRow(piecePosition.position.col + 1, piecePosition.position.row);
        var neighbourRight = boardState.GetPieceTypeAtPosition(neighbourRightPosition);
        if (boardState.EnPassantColumn == neighbourRightPosition.getCol() && neighbourRight.IsWhite() != piecePosition.pieceType.IsWhite() && neighbourRight.IsPawn())
        {
          moves.Add(new Move(piecePosition.position, captureRightPosition, PieceType.Nothing));
        }
      }
    }

    if (piecePosition.position.row + increment != stopCondition)
    {
      return moves;
    }

    var promotionMoves = new List<Move>(moves.Count * 4);
    var promotions = piecePosition.pieceType.IsWhite()
        ? new PieceType[] { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
        : new PieceType[] { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

    // Multiply moves by promotions
    for (var moveIndex = 0; moveIndex < moves.Count; ++moveIndex)
    {
      for (var promotionIndex = 0; promotionIndex < promotions.Length; ++promotionIndex)
      {
        promotionMoves.Add(new Move(piecePosition.position, moves[moveIndex].target, promotions[promotionIndex]));
      }
    }

    return promotionMoves;
  }

  private static List<Move> GetPseudoLegalRookMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    return GetPseudoRayMoves(boardState, piecePosition.position, 1, 0, piecePosition.pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 0, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, -1, piecePosition.pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalKnightMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    var validMoves = new List<Move>(8);
    var jumps = V8Precomputed.knightJumps[piecePosition.position.index];

    for (var jumpIndex = 0; jumpIndex < jumps.Length; ++jumpIndex)
    {
      var jump = jumps[jumpIndex];
      var collision = boardState.GetPieceTypeAtPosition(jump);
      if (collision != PieceType.Nothing && collision.IsWhite() == piecePosition.pieceType.IsWhite()) continue;
      validMoves.Add(new Move(piecePosition.position, jump.toBoardPosition(), PieceType.Nothing));
    }

    return validMoves;
  }

  private static List<Move> GetPseudoLegalBishopMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    return GetPseudoRayMoves(boardState, piecePosition.position, 1, 1, piecePosition.pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, -1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 1, -1, piecePosition.pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalQueenMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    return GetPseudoRayMoves(boardState, piecePosition.position, 1, 0, piecePosition.pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 0, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, -1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 1, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, -1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 1, -1, piecePosition.pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalKingMoves(V8BoardState boardState, PiecePosition piecePosition)
  {
    var landings = new[]{
      new { col = piecePosition.position.col - 1, row = piecePosition.position.row - 1},
      new { col = piecePosition.position.col - 1, row = piecePosition.position.row    },
      new { col = piecePosition.position.col - 1, row = piecePosition.position.row + 1},
      new { col = piecePosition.position.col    , row = piecePosition.position.row + 1},
      new { col = piecePosition.position.col + 1, row = piecePosition.position.row + 1},
      new { col = piecePosition.position.col + 1, row = piecePosition.position.row    },
      new { col = piecePosition.position.col + 1, row = piecePosition.position.row - 1},
      new { col = piecePosition.position.col    , row = piecePosition.position.row - 1},
    };

    var normalMoves = landings.Where(landing =>
    {
      if (!BoardPosition.IsInBoard(landing.col, landing.row)) return false;

      var collision = boardState.GetPieceTypeAtPosition(BoardPosition.fromColRow(landing.col, landing.row));

      if (collision == PieceType.Nothing) return true;

      return collision.IsWhite() != piecePosition.pieceType.IsWhite();
    }).Select(landing => new Move(piecePosition.position, new BoardPosition(landing.col, landing.row), PieceType.Nothing))
    .ToList();

    var castles = new[] {
      new { castle = CastleFlags.WhiteKing, emptyPositions = new List<int>{ BoardPosition.fromColRow(5, 0), BoardPosition.fromColRow(6, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 0), new BoardPosition(6, 0) } },
      new { castle = CastleFlags.WhiteQueen, emptyPositions = new List<int>{ BoardPosition.fromColRow(3, 0), BoardPosition.fromColRow(2, 0), BoardPosition.fromColRow(1, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 0), new BoardPosition(2, 0) } },
      new { castle = CastleFlags.BlackKing, emptyPositions = new List<int>{ BoardPosition.fromColRow(5, 7), BoardPosition.fromColRow(6, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 7), new BoardPosition(6, 7) } },
      new { castle = CastleFlags.BlackQueen, emptyPositions = new List<int>{ BoardPosition.fromColRow(3, 7), BoardPosition.fromColRow(2, 7), BoardPosition.fromColRow(1, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 7), new BoardPosition(2, 7) } },
    };

    var rockMoves = castles.Where(rock =>
    {
      if (!boardState.CastleFlags.HasFlag(rock.castle)) return false;
      if (piecePosition.pieceType.IsWhite() != rock.castle.IsWhite()) return false;

      foreach (var emptyPosition in rock.emptyPositions)
      {
        if (boardState.GetPieceTypeAtPosition(emptyPosition) != PieceType.Nothing) return false;
      }

      var lastCheckedBoardState = boardState;
      var lastKingPosition = piecePosition.position;

      if (CanKingDie(lastCheckedBoardState, piecePosition.pieceType.IsWhite())) return false;

      foreach (var noCheckPosition in rock.noCheckPositions)
      {
        lastCheckedBoardState = lastCheckedBoardState.PlayMove(new Move(lastKingPosition, noCheckPosition, PieceType.Nothing)).boardState;
        lastKingPosition = noCheckPosition;
        var canKingDie = CanKingDie(lastCheckedBoardState, piecePosition.pieceType.IsWhite());
        if (canKingDie) return false;
      }

      return true;
    }).Select(rock => new Move(piecePosition.position, rock.noCheckPositions[^1], PieceType.Nothing)).ToList();

    return normalMoves.Concat(rockMoves).ToList();
  }

  private static List<Move> GetPseudoRayMoves(V8BoardState boardState, BoardPosition position, int colIncrement, int rowIncrement, bool isWhite)
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

  private static V8PiecePosition GetRayNextPiece(V8BoardState boardState, BoardPosition position, int colIncrement, int rowIncrement)
  {
    var col = position.col + colIncrement;
    var row = position.row + rowIncrement;
    while (col <= 7 && col >= 0 && row <= 7 && row >= 0)
    {
      var currentPosition = BoardPosition.fromColRow(col, row);
      var foundCollision = boardState.GetPieceTypeAtPosition(currentPosition);

      if (foundCollision != PieceType.Nothing) return new V8PiecePosition(foundCollision, currentPosition);

      col += colIncrement;
      row += rowIncrement;
    }

    return null;
  }
}