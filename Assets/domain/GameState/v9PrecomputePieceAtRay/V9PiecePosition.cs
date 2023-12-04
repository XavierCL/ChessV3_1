public class V9PiecePosition
{
  public readonly PieceType pieceType;
  public readonly int position;

  public V9PiecePosition(
    PieceType pieceType,
    int position
  )
  {
    this.pieceType = pieceType;
    this.position = position;
  }
}