using System.Collections.Generic;
using System.Linq;

public static class LegalMoveGenerator
{
  public static List<Move> GenerateLegalMoves(this GameState gameState)
  {
    var pseudoLegalMoves = GeneratePseudoLegalMoves(gameState, gameState.whiteTurn);
    var checkValidationState = new GameState(gameState);

    return pseudoLegalMoves.Where(move => !CanOwnKingDieAfterMove(checkValidationState, move)).ToList();
  }

  public static bool CanOwnKingDie(GameState gameState)
  {
    return CanKingDie(new GameState(gameState), gameState.whiteTurn);
  }

  private static bool CanOwnKingDieAfterMove(GameState gameState, Move ownMove)
  {
    gameState.PlayMove(ownMove);
    var canOwnKingDieNextMove = CanKingDie(gameState, !gameState.whiteTurn);
    gameState.UndoMove();

    return canOwnKingDieNextMove;
  }

  private static bool CanKingDie(GameState gameState, bool whiteKing)
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

  private static List<Move> GeneratePseudoLegalMoves(GameState gameState, bool white)
  {
    return gameState.piecePositions
      .Where(piecePosition => piecePosition.pieceType.IsWhite() == white)
      .SelectMany(piecePosition => GetPseudoLegalMoves(gameState, piecePosition))
      .ToList();
  }

  private static List<Move> GetPseudoLegalMoves(GameState gameState, PiecePosition piecePosition)
  {
    switch (piecePosition.pieceType)
    {
      case PieceType.WhitePawn:
      case PieceType.BlackPawn:
        return GetPseudoLegalPawnMoves(piecePosition);
      default:
        return new List<Move>();
    }
  }

  private static List<Move> GetPseudoLegalPawnMoves(PiecePosition piecePosition)
  {
    var ownPawnStartingY = piecePosition.pieceType.IsWhite() ? 1 : 6;
    var increment = piecePosition.pieceType.IsWhite() ? 1 : -1;
    var stopCondition = piecePosition.pieceType.IsWhite() ? 7 : 0;
    var promotions = piecePosition.pieceType.IsWhite()
        ? new List<PieceType> { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
        : new List<PieceType> { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

    // Starting position can move up two rows
    if (piecePosition.position.row == ownPawnStartingY)
    {
      return new List<Move> {
        new Move(piecePosition.position, new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment), PieceType.Nothing),
        new Move(piecePosition.position, new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment * 2), PieceType.Nothing)
      };
    }

    // Promotions
    if (piecePosition.position.row == stopCondition - increment)
    {
      return promotions
        .Select(promotion => new Move(piecePosition.position, new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment), promotion))
        .ToList();
    }

    // Normal 1 square move
    return new List<Move> { new Move(piecePosition.position, new BoardPosition(piecePosition.position.col, piecePosition.position.row + increment), PieceType.Nothing) };
  }
}