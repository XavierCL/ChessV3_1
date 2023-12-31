
using System.Diagnostics;

[DebuggerDisplay("{pieceType} {position.pretty}")]
public class PiecePosition
{
  public readonly string id;
  public readonly PieceType pieceType;
  public readonly BoardPosition position;

  public PiecePosition(
    string id,
    PieceType pieceType,
    BoardPosition position
  )
  {
    this.id = id;
    this.pieceType = pieceType;
    this.position = position;
  }

  public PiecePosition(PiecePosition piecePosition)
  {
    id = piecePosition.id;
    pieceType = piecePosition.pieceType;
    position = piecePosition.position;
  }

  public PiecePosition PlayMove(BoardPosition targetPosition, PieceType promotion)
  {
    return new PiecePosition(id, promotion == PieceType.Nothing ? pieceType : promotion, targetPosition);
  }

  // Equality code doesn't check the id. Game engine doesn't hold an id per piece.
  public override int GetHashCode()
  {
    unchecked
    {
      return pieceType.GetHashCode() * 64 + position.GetHashCode();
    }
  }

  // Equality code doesn't check the id. Game engine doesn't hold an id per piece.
  public override bool Equals(object obj)
  {
    var other = (PiecePosition)obj;
    return pieceType == other.pieceType && position.Equals(other.position);
  }
}
