public class V8PiecePosition
{
  public readonly PieceType pieceType;
  public readonly int position;

  public V8PiecePosition(
    PieceType pieceType,
    int position
  )
  {
    this.pieceType = pieceType;
    this.position = position;
  }
}