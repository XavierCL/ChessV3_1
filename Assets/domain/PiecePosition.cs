public class PiecePosition
{
    public string id { get; }
    public PieceType pieceType { get; set; }
    public BoardPosition position { get; set; }

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
}
