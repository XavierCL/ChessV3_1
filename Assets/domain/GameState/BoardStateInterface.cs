
using System.Collections.Generic;

public interface BoardStateInterface
{
  public bool whiteTurn { get; }
  public List<PiecePosition> piecePositions { get; }
  public CastleFlags castleFlags { get; }
  public int enPassantColumn { get; }
}
