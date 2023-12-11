
using System.Collections.Generic;

public interface BoardStateInterface
{
  public bool whiteTurn { get; }
  public List<PiecePosition> piecePositions { get; }
  public bool whiteCastleKingSide { get; }
  public bool whiteCastleQueenSide { get; }
  public bool blackCastleKingSide { get; }
  public bool blackCastleQueenSide { get; }
  public int enPassantColumn { get; }
}
