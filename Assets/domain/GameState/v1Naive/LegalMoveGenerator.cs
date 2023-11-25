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
    return gameState.piecePositions
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
      default:
        return new List<Move>();
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