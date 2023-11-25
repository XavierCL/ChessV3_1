using System.Collections.Generic;
using System.Linq;

public static class LegalMoveGenerator
{
  public static List<Move> GenerateLegalMoves(this V1GameState gameState)
  {
    var pseudoLegalMoves = GeneratePseudoLegalMoves(gameState, gameState.whiteTurn);
    var checkValidationState = new V1GameState(gameState);

    return pseudoLegalMoves.Where(move => !CanOwnKingDieAfterMove(checkValidationState, move)).ToList();
  }

  public static bool CanOwnKingDie(V1GameState gameState)
  {
    return CanKingDie(new V1GameState(gameState), gameState.whiteTurn);
  }

  private static bool CanOwnKingDieAfterMove(V1GameState gameState, Move ownMove)
  {
    gameState.PlayMove(ownMove);
    var canOwnKingDieNextMove = CanKingDie(gameState, !gameState.whiteTurn);
    gameState.UndoMove();

    return canOwnKingDieNextMove;
  }

  private static bool CanKingDie(V1GameState gameState, bool whiteKing)
  {
    var nextPseudoLegalMoves = GeneratePseudoLegalMoves(gameState, !whiteKing);
    return nextPseudoLegalMoves.Any(pseudoMove =>
    {
      gameState.PlayMove(pseudoMove);
      var hasOwnKing = gameState.HasKing(whiteKing);
      gameState.UndoMove();
      return !hasOwnKing;
    });
  }

  private static List<Move> GeneratePseudoLegalMoves(V1GameState gameState, bool white)
  {
    var piecePositions = new List<PiecePosition>(gameState.piecePositions);

    return piecePositions
      .Where(piecePosition => piecePosition.pieceType.IsWhite() == white)
      .SelectMany(piecePosition => GetPseudoLegalMoves(gameState, piecePosition))
      .ToList();
  }

  private static List<Move> GetPseudoLegalMoves(V1GameState gameState, PiecePosition piecePosition)
  {
    switch (piecePosition.pieceType)
    {
      case PieceType.WhitePawn:
      case PieceType.BlackPawn:
        return GetPseudoLegalPawnMoves(gameState, piecePosition);
      case PieceType.WhiteRook:
      case PieceType.BlackRook:
        return GetPseudoLegalRookMoves(gameState, piecePosition);
      case PieceType.WhiteKnight:
      case PieceType.BlackKnight:
        return GetPseudoLegalKnightMoves(gameState, piecePosition);
      case PieceType.WhiteBishop:
      case PieceType.BlackBishop:
        return GetPseudoLegalBishopMoves(gameState, piecePosition);
      case PieceType.WhiteQueen:
      case PieceType.BlackQueen:
        return GetPseudoLegalQueenMoves(gameState, piecePosition);
      case PieceType.WhiteKing:
      case PieceType.BlackKing:
        return GetPseudoLegalKingMoves(gameState, piecePosition);
      default:
        throw new System.Exception("Not a valid piece");
    }
  }

  private static List<Move> GetPseudoLegalPawnMoves(V1GameState gameState, PiecePosition piecePosition)
  {
    var ownPawnStartingY = piecePosition.pieceType.IsWhite() ? 1 : 6;
    var increment = piecePosition.pieceType.IsWhite() ? 1 : -1;
    var stopCondition = piecePosition.pieceType.IsWhite() ? 7 : 0;
    var promotions = piecePosition.pieceType.IsWhite()
        ? new List<PieceType> { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
        : new List<PieceType> { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

    var moves = new List<Move>();

    var oneUpPosition = new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment);
    if (!gameState.piecePositions.Any(piece => piece.position.Equals(oneUpPosition)))
    {
      moves.Add(new Move(piecePosition.position, oneUpPosition, PieceType.Nothing));
    }

    var twoUpPosition = new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment * 2);
    if (piecePosition.position.row == ownPawnStartingY && !gameState.piecePositions.Any(piece => piece.position.Equals(twoUpPosition)))
    {
      moves.Add(new Move(piecePosition.position, twoUpPosition, PieceType.Nothing));
    }

    var captureLeftPosition = new BoardPosition(piecePosition.position.col - 1, piecePosition.position.row + increment);
    if (piecePosition.position.col != 0 && gameState.piecePositions.Any(piece => piece.pieceType.IsWhite() != piecePosition.pieceType.IsWhite() && piece.position.Equals(captureLeftPosition)))
    {
      moves.Add(new Move(piecePosition.position, captureLeftPosition, PieceType.Nothing));
    }

    var captureRightPosition = new BoardPosition(piecePosition.position.col + 1, piecePosition.position.row + increment);
    if (piecePosition.position.col != 7 && gameState.piecePositions.Any(piece => piece.pieceType.IsWhite() != piecePosition.pieceType.IsWhite() && piece.position.Equals(captureRightPosition)))
    {
      moves.Add(new Move(piecePosition.position, captureRightPosition, PieceType.Nothing));
    }

    // En passant
    var neighbourLeftPosition = new BoardPosition(piecePosition.position.col - 1, piecePosition.position.row);
    if (piecePosition.position.col != 0 && gameState.history.Count > 0 && gameState.piecePositions.Any(piece => piece.pieceType.IsWhite() != piecePosition.pieceType.IsWhite() && piece.pieceType.IsPawn() && piece.position.Equals(neighbourLeftPosition)))
    {
      var lastHistory = gameState.history[^1];
      if (lastHistory.target.Equals(neighbourLeftPosition) && lastHistory.target.row == lastHistory.source.row - increment * 2)
      {
        moves.Add(new Move(piecePosition.position, captureLeftPosition, PieceType.Nothing));
      }
    }

    var neighbourRightPosition = new BoardPosition(piecePosition.position.col + 1, piecePosition.position.row);
    if (piecePosition.position.col != 7 && gameState.history.Count > 0 && gameState.piecePositions.Any(piece => piece.pieceType.IsWhite() != piecePosition.pieceType.IsWhite() && piece.pieceType.IsPawn() && piece.position.Equals(neighbourRightPosition)))
    {
      var lastHistory = gameState.history[^1];
      if (lastHistory.target.Equals(neighbourRightPosition) && lastHistory.target.row == lastHistory.source.row - increment * 2)
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

  private static List<Move> GetPseudoLegalRookMoves(V1GameState gameState, PiecePosition piecePosition)
  {
    return GetPseudoRayMoves(gameState, piecePosition.position, 1, 0, piecePosition.pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, -1, 0, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 0, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 0, -1, piecePosition.pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalKnightMoves(V1GameState gameState, PiecePosition piecePosition)
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

      var collision = gameState.piecePositions.Find(piece => piece.position.Equals(new BoardPosition(jump.col, jump.row)));

      if (collision == null) return true;

      return collision.pieceType.IsWhite() != piecePosition.pieceType.IsWhite();
    }).Select(jump => new Move(piecePosition.position, new BoardPosition(jump.col, jump.row), PieceType.Nothing))
    .ToList();
  }

  private static List<Move> GetPseudoLegalBishopMoves(V1GameState gameState, PiecePosition piecePosition)
  {
    return GetPseudoRayMoves(gameState, piecePosition.position, 1, 1, piecePosition.pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, -1, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, -1, -1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 1, -1, piecePosition.pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalQueenMoves(V1GameState gameState, PiecePosition piecePosition)
  {
    return GetPseudoRayMoves(gameState, piecePosition.position, 1, 0, piecePosition.pieceType.IsWhite())
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, -1, 0, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 0, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 0, -1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 1, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, -1, 1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, -1, -1, piecePosition.pieceType.IsWhite()))
      .Concat(GetPseudoRayMoves(gameState, piecePosition.position, 1, -1, piecePosition.pieceType.IsWhite())).ToList();
  }

  private static List<Move> GetPseudoLegalKingMoves(V1GameState gameState, PiecePosition piecePosition)
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

      var collision = gameState.piecePositions.Find(piece => piece.position.Equals(new BoardPosition(landing.col, landing.row)));

      if (collision == null) return true;

      return collision.pieceType.IsWhite() != piecePosition.pieceType.IsWhite();
    }).Select(landing => new Move(piecePosition.position, new BoardPosition(landing.col, landing.row), PieceType.Nothing))
    .ToList();

    var rocks = new[] {
      new { isWhite = true, canRock = gameState.whiteCastleKingSide, emptyPositions = new List<BoardPosition>{ new BoardPosition(5, 0), new BoardPosition(6, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 0), new BoardPosition(6, 0) } },
      new { isWhite = true, canRock = gameState.whiteCastleQueenSide, emptyPositions = new List<BoardPosition>{ new BoardPosition(3, 0), new BoardPosition(2, 0), new BoardPosition(1, 0) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 0), new BoardPosition(2, 0) } },
      new { isWhite = false, canRock = gameState.blackCastleKingSide, emptyPositions = new List<BoardPosition>{ new BoardPosition(5, 7), new BoardPosition(6, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(5, 7), new BoardPosition(6, 7) } },
      new { isWhite = false, canRock = gameState.blackCastleQueenSide, emptyPositions = new List<BoardPosition>{ new BoardPosition(3, 7), new BoardPosition(2, 7), new BoardPosition(1, 7) }, noCheckPositions = new List<BoardPosition>{ new BoardPosition(3, 7), new BoardPosition(2, 7) } },
    };

    var rockMoves = rocks.Where(rock =>
    {
      if (!rock.canRock) return false;
      if (rock.isWhite != piecePosition.pieceType.IsWhite()) return false;

      foreach (var emptyPosition in rock.emptyPositions)
      {
        if (gameState.piecePositions.Any(piece => piece.position.Equals(emptyPosition))) return false;
      }

      var whiteRockKingSide = gameState.whiteCastleKingSide;
      var whiteRockQueenSide = gameState.whiteCastleQueenSide;
      var blackRockKingSide = gameState.blackCastleKingSide;
      var blackRockQueenSide = gameState.blackCastleQueenSide;

      // Disable rocks whilst doing a rock king dies check to avoid infinite loop.
      gameState.whiteCastleKingSide = false;
      gameState.whiteCastleQueenSide = false;
      gameState.blackCastleKingSide = false;
      gameState.blackCastleQueenSide = false;

      try
      {
        if (CanKingDie(gameState, rock.isWhite)) return false;

        foreach (var noCheckPosition in rock.noCheckPositions)
        {
          gameState.SimplePieceMove(new Move(piecePosition.position, noCheckPosition, PieceType.Nothing));
          var canKingDie = CanKingDie(gameState, rock.isWhite);
          gameState.SimplePieceMove(new Move(noCheckPosition, piecePosition.position, PieceType.Nothing));
          if (canKingDie) return false;
        }
      }
      finally
      {
        gameState.whiteCastleKingSide = whiteRockKingSide;
        gameState.whiteCastleQueenSide = whiteRockQueenSide;
        gameState.blackCastleKingSide = blackRockKingSide;
        gameState.blackCastleQueenSide = blackRockQueenSide;
      }

      return true;
    }).Select(rock => new Move(piecePosition.position, rock.noCheckPositions[^1], PieceType.Nothing)).ToList();

    return normalMoves.Concat(rockMoves).ToList();
  }

  private static List<Move> GetPseudoRayMoves(V1GameState gameState, BoardPosition position, int colIncrement, int rowIncrement, bool isWhite)
  {
    var moves = new List<Move>();
    var col = position.col + colIncrement;
    var row = position.row + rowIncrement;
    while (col <= 7 && col >= 0 && row <= 7 && row >= 0)
    {
      var foundCollision = gameState.piecePositions.Find(piece => piece.position.Equals(new BoardPosition(col, row)));

      if (foundCollision != null && foundCollision.pieceType.IsWhite() == isWhite) return moves;

      moves.Add(new Move(position, new BoardPosition(col, row), PieceType.Nothing));

      if (foundCollision != null) return moves;

      col += colIncrement;
      row += rowIncrement;
    }

    return moves;
  }
}