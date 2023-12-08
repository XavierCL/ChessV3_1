using System.Linq;

public static class Ai2Evaluate
{
  public static double Evaluate(V9GameState gameState)
  {
    var endGameState = gameState.GetGameEndState();

    if (endGameState == GameEndState.WhiteWin)
    {
      return double.MaxValue;
    }
    else if (endGameState == GameEndState.BlackWin)
    {
      return double.MinValue;
    }
    else if (endGameState == GameEndState.Draw)
    {
      return 0;
    }

    return gameState.boardState.piecePositions.Aggregate(0.0, (agg, cur) => agg + EvaluatePiece(cur.pieceType));
  }

  public static double EvaluatePiece(PieceType pieceType)
  {
    double value = 0;
    if (pieceType.IsPawn())
    {
      value = 1;
    }
    else if (pieceType.IsRook())
    {
      value = 5;
    }
    else if (pieceType.IsKnight())
    {
      value = 3;
    }
    else if (pieceType.IsBishop())
    {
      value = 3;
    }
    else if (pieceType.IsQueen())
    {
      value = 9;
    }

    if (pieceType.IsBlack()) value = -value;

    return value;
  }
}