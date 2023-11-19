public class Move
{
    public BoardPosition source { get; }
    public BoardPosition target { get; }
    public PieceType promotion { get; }

    public Move(BoardPosition source, BoardPosition target, PieceType promotion)
    {
        this.source = source;
        this.target = target;
        this.promotion = promotion;
    }
}
