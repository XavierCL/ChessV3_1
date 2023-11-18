public class Move
{
    public BoardPosition source { get; set; }
    public BoardPosition target { get; set; }
    public PieceType promotion { get; set; }

    public Move(BoardPosition source, BoardPosition target, PieceType promotion)
    {
        this.source = source;
        this.target = target;
        this.promotion = promotion;
    }
}
