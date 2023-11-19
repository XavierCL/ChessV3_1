using System.Collections.Generic;
using System.Linq;

public static class LegalMoveGenerator
{
  public static List<Move> GenerateLegalMoves(this GameState gameState)
  {
    var pseudoLegalMoves = GeneratePseudoLegalMoves(gameState);
    var checkValidationState = new GameState(gameState);

    return pseudoLegalMoves.Where(move =>
    {
      checkValidationState.PlayMove(move);
      var nextPseudoLegalMoves = GeneratePseudoLegalMoves(checkValidationState);
      // check if king is dead
    }).ToList();
  }

  private static List<Move> GeneratePseudoLegalMoves(GameState gameState)
  {
    return gameState.piecePositions.SelectMany(piecePosition => GetPseudoLegalMoves(gameState, piecePosition)).ToList();
  }

  private static List<Move> GetPseudoLegalMoves(GameState gameState, PiecePosition piecePosition)
  {

  }
}