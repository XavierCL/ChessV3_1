using System.Collections.Generic;
using System.Linq;

public static class V2LegalMoveGenerator
{
  /// <summary>
  /// Doesn't take end game draws in consideration (three-fold nor 50 moves)
  /// </summary>
  public static List<Move> GenerateLegalMoves(this V2GameState gameState)
  {
    var pseudoLegalMoves = GeneratePseudoLegalMoves(gameState.boardState, gameState.BoardState.WhiteTurn);
    return pseudoLegalMoves.Where(move => !CanKingDieAfterMove(gameState.boardState, move, gameState.BoardState.WhiteTurn)).ToList();
  }

  public static bool CanOwnKingDie(V2GameState gameState)
  {
    return CanKingDie(gameState.boardState, gameState.BoardState.WhiteTurn);
  }

  private static bool CanKingDieAfterMove(V2BoardState boardState, Move ownMove, bool whiteKing)
  {
    var kingMightBeInCheckState = boardState.PlayMove(ownMove).boardState;
    return CanKingDie(kingMightBeInCheckState, whiteKing);
  }

  private static bool CanKingDie(V2BoardState boardState, bool whiteKing)
  {
    var nextPseudoLegalMoves = GeneratePseudoLegalMoves(boardState, !whiteKing, true);
    return nextPseudoLegalMoves.Any(pseudoMove =>
    {
      var kingMightHaveDiedState = boardState.PlayMove(pseudoMove).boardState;
      var hasKing = kingMightHaveDiedState.HasKing(whiteKing);
      return !hasKing;
    });
  }

  private static List<Move> GeneratePseudoLegalMoves(V2BoardState boardState, bool white, bool canKillKingOnly = false)
  {
    var piecePositions = new List<PiecePosition>(boardState.piecePositions);

    return piecePositions
      .Where(piecePosition => piecePosition.pieceType.IsWhite() == white)
      .SelectMany(piecePosition => GetPseudoLegalMoves(boardState, piecePosition, canKillKingOnly))
      .ToList();
  }

  private static List<Move> GetPseudoLegalMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
  {
    switch (piecePosition.pieceType)
    {
      case PieceType.WhitePawn:
      case PieceType.BlackPawn:
        return GetPseudoLegalPawnMoves(boardState, piecePosition, canKillKingOnly);
      case PieceType.WhiteRook:
      case PieceType.BlackRook:
        return GetPseudoLegalRookMoves(boardState, piecePosition, canKillKingOnly);
      case PieceType.WhiteKnight:
      case PieceType.BlackKnight:
        return GetPseudoLegalKnightMoves(boardState, piecePosition, canKillKingOnly);
      case PieceType.WhiteBishop:
      case PieceType.BlackBishop:
        return GetPseudoLegalBishopMoves(boardState, piecePosition, canKillKingOnly);
      case PieceType.WhiteQueen:
      case PieceType.BlackQueen:
        return GetPseudoLegalQueenMoves(boardState, piecePosition, canKillKingOnly);
      case PieceType.WhiteKing:
      case PieceType.BlackKing:
        return GetPseudoLegalKingMoves(boardState, piecePosition, canKillKingOnly);
      default:
        throw new System.Exception("Not a valid piece");
    }
  }

  private static List<Move> GetPseudoLegalPawnMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
  {
    var ownPawnStartingY = piecePosition.pieceType.IsWhite() ? 1 : 6;
    var increment = piecePosition.pieceType.IsWhite() ? 1 : -1;
    var stopCondition = piecePosition.pieceType.IsWhite() ? 7 : 0;
    var promotions = piecePosition.pieceType.IsWhite()
        ? new List<PieceType> { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
        : new List<PieceType> { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

    var moves = new List<Move>();

    var oneUpPosition = new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment);
    var oneUpPieceExists = boardState.GetPieceTypeAtPosition(oneUpPosition) != PieceType.Nothing;
    if (!oneUpPieceExists && !canKillKingOnly)
    {
      moves.Add(new Move(piecePosition.position, oneUpPosition, PieceType.Nothing));
    }

    var twoUpPosition = new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment * 2);
    if (piecePosition.position.row == ownPawnStartingY && !oneUpPieceExists && !canKillKingOnly && boardState.GetPieceTypeAtPosition(twoUpPosition) == PieceType.Nothing)
    {
      moves.Add(new Move(piecePosition.position, twoUpPosition, PieceType.Nothing));
    }

    var captureLeftPosition = new BoardPosition(piecePosition.position.col - 1, piecePosition.position.row + increment);
    var captureLeft = boardState.GetPieceTypeAtPosition(captureLeftPosition);
    if (piecePosition.position.col != 0 && captureLeft != PieceType.Nothing && captureLeft.IsWhite() != piecePosition.pieceType.IsWhite())
    {
      if (captureLeft.IsKing() || !canKillKingOnly)
      {
        moves.Add(new Move(piecePosition.position, captureLeftPosition, PieceType.Nothing));
      }
    }

    var captureRightPosition = new BoardPosition(piecePosition.position.col + 1, piecePosition.position.row + increment);
    var captureRight = boardState.GetPieceTypeAtPosition(captureRightPosition);
    if (piecePosition.position.col != 7 && captureRight != PieceType.Nothing && captureRight.IsWhite() != piecePosition.pieceType.IsWhite())
    {
      if (captureRight.IsKing() || !canKillKingOnly)
      {
        moves.Add(new Move(piecePosition.position, captureRightPosition, PieceType.Nothing));
      }
    }

    // En passant
    var enemyFourthRow = piecePosition.pieceType.IsWhite() ? 4 : 3;
    if (piecePosition.position.row == enemyFourthRow)
    {
      var neighbourLeftPosition = new BoardPosition(piecePosition.position.col - 1, piecePosition.position.row);
      var neighbourLeft = boardState.GetPieceTypeAtPosition(neighbourLeftPosition);
      if (piecePosition.position.col != 0 && boardState.EnPassantColumn == neighbourLeftPosition.col && !canKillKingOnly && neighbourLeft.IsWhite() != piecePosition.pieceType.IsWhite() && neighbourLeft.IsPawn())
      {
        moves.Add(new Move(piecePosition.position, captureLeftPosition, PieceType.Nothing));
      }

      var neighbourRightPosition = new BoardPosition(piecePosition.position.col + 1, piecePosition.position.row);
      var neighbourRight = boardState.GetPieceTypeAtPosition(neighbourRightPosition);
      if (piecePosition.position.col != 7 && boardState.EnPassantColumn == neighbourRightPosition.col && !canKillKingOnly && neighbourRight.IsWhite() != piecePosition.pieceType.IsWhite() && neighbourRight.IsPawn())
      {
        moves.Add(new Move(piecePosition.position, captureRightPosition, PieceType.Nothing));
      }
    }

    // Multiply moves by promotions
    if (piecePosition.position.row + increment == stopCondition)
    {
      moves = moves.SelectMany(noPromotionMove => promotions.Select(promotion => new Move(piecePosition.position, noPromotionMove.target, promotion))).ToList();
    }

    return moves;
  }

  private static List<Move> GetPseudoLegalRookMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
  {
    return GetPseudoRayMoves(boardState, piecePosition.position, 1, 0, piecePosition.pieceType.IsWhite(), canKillKingOnly)
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 0, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, 1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, -1, piecePosition.pieceType.IsWhite(), canKillKingOnly)).ToList();
  }

  private static List<Move> GetPseudoLegalKnightMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
  {
    var jumps = new[]{
      new { col = piecePosition.position.col - 2, row = piecePosition.position.row - 1},
      new { col = piecePosition.position.col - 2, row = piecePosition.position.row + 1},
      new { col = piecePosition.position.col - 1, row = piecePosition.position.row + 2},
      new { col = piecePosition.position.col + 1, row = piecePosition.position.row + 2},
      new { col = piecePosition.position.col + 2, row = piecePosition.position.row + 1},
      new { col = piecePosition.position.col + 2, row = piecePosition.position.row - 1},
      new { col = piecePosition.position.col - 1, row = piecePosition.position.row - 2},
      new { col = piecePosition.position.col + 1, row = piecePosition.position.row - 2},
    };

    return jumps.Where(jump =>
    {
      if (!BoardPosition.IsInBoard(jump.col, jump.row)) return false;

      var collision = boardState.GetPieceTypeAtPosition(new BoardPosition(jump.col, jump.row));

      if (collision == PieceType.Nothing) return true;

      if (collision.IsWhite() == piecePosition.pieceType.IsWhite()) return false;

      return collision.IsKing() || !canKillKingOnly;
    }).Select(jump => new Move(piecePosition.position, new BoardPosition(jump.col, jump.row), PieceType.Nothing))
    .ToList();
  }

  private static List<Move> GetPseudoLegalBishopMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
  {
    return GetPseudoRayMoves(boardState, piecePosition.position, 1, 1, piecePosition.pieceType.IsWhite(), canKillKingOnly)
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, -1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 1, -1, piecePosition.pieceType.IsWhite(), canKillKingOnly)).ToList();
  }

  private static List<Move> GetPseudoLegalQueenMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
  {
    return GetPseudoRayMoves(boardState, piecePosition.position, 1, 0, piecePosition.pieceType.IsWhite(), canKillKingOnly)
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 0, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, 1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 0, -1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 1, 1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, 1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, -1, -1, piecePosition.pieceType.IsWhite(), canKillKingOnly))
      .Concat(GetPseudoRayMoves(boardState, piecePosition.position, 1, -1, piecePosition.pieceType.IsWhite(), canKillKingOnly)).ToList();
  }

  private static List<Move> GetPseudoLegalKingMoves(V2BoardState boardState, PiecePosition piecePosition, bool canKillKingOnly)
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

      var collision = boardState.GetPieceTypeAtPosition(new BoardPosition(landing.col, landing.row));

      if (collision == PieceType.Nothing) return true;

      if (collision.IsWhite() == piecePosition.pieceType.IsWhite()) return false;

      return collision.IsKing() || !canKillKingOnly;
    }).Select(landing => new Move(piecePosition.position, new BoardPosition(landing.col, landing.row), PieceType.Nothing))
    .ToList();

    if (canKillKingOnly) return normalMoves;

    var castles = new[] {
      new { castle = CastleFlags.WhiteKing, emptyPositions = new List<BoardPosition>{ new BoardPosition(5, 0), new BoardPosition(6, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 0), new BoardPosition(6, 0) } },
      new { castle = CastleFlags.WhiteQueen, emptyPositions = new List<BoardPosition>{ new BoardPosition(3, 0), new BoardPosition(2, 0), new BoardPosition(1, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 0), new BoardPosition(2, 0) } },
      new { castle = CastleFlags.BlackKing, emptyPositions = new List<BoardPosition>{ new BoardPosition(5, 7), new BoardPosition(6, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 7), new BoardPosition(6, 7) } },
      new { castle = CastleFlags.BlackQueen, emptyPositions = new List<BoardPosition>{ new BoardPosition(3, 7), new BoardPosition(2, 7), new BoardPosition(1, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 7), new BoardPosition(2, 7) } },
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

  private static List<Move> GetPseudoRayMoves(V2BoardState boardState, BoardPosition position, int colIncrement, int rowIncrement, bool isWhite, bool canKillKingOnly)
  {
    var moves = new List<Move>();
    var col = position.col + colIncrement;
    var row = position.row + rowIncrement;
    while (col <= 7 && col >= 0 && row <= 7 && row >= 0)
    {
      var foundCollision = boardState.GetPieceTypeAtPosition(new BoardPosition(col, row));

      if (foundCollision != PieceType.Nothing && foundCollision.IsWhite() == isWhite) return moves;

      if (!canKillKingOnly || foundCollision != PieceType.Nothing && foundCollision.IsKing())
      {
        moves.Add(new Move(position, new BoardPosition(col, row), PieceType.Nothing));
      }

      if (foundCollision != PieceType.Nothing) return moves;

      col += colIncrement;
      row += rowIncrement;
    }

    return moves;
  }
}