using System.Collections.Generic;
using System.Linq;

public static class LegalMoveGenerator
{
  public static List<Move> GenerateLegalMoves(this GameState gameState)
  {
    var pseudoLegalMoves = GeneratePseudoLegalMoves(gameState);
    var checkValidationState = new GameState(gameState);

    return pseudoLegalMoves.Where(move => CanOwnKingDieNextTurn(checkValidationState, move)).ToList();
  }

  private static bool CanOwnKingDieNextTurn(GameState gameState, Move ownMove)
  {
    gameState.PlayMove(ownMove);
    var nextPseudoLegalMoves = GeneratePseudoLegalMoves(gameState);
    var canOwnKingDieNextMove = nextPseudoLegalMoves.Any(pseudoMove =>
    {
      gameState.PlayMove(pseudoMove);
      var hasOwnKing = gameState.HasKing(gameState.whiteTurn);
      gameState.UndoMove();
      return !hasOwnKing;
    });
    gameState.UndoMove();

    return canOwnKingDieNextMove
  }

  private static List<Move> GeneratePseudoLegalMoves(GameState gameState)
  {
    return gameState.piecePositions.SelectMany(piecePosition => GetPseudoLegalMoves(gameState, piecePosition)).ToList();
  }

  private static List<Move> GetPseudoLegalMoves(GameState gameState, PiecePosition piecePosition)
  {

  }
}