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

    public bool Equals(Move otherMove)
    {
        return Equals(source, otherMove.source) && Equals(target, otherMove.target) && Equals(promotion, otherMove.promotion);
    }
}
