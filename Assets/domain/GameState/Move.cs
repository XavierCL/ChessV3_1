using System.Diagnostics;

[DebuggerDisplay("{source.pretty}{target.pretty}")]
public class Move
{
    public readonly BoardPosition source;
    public readonly BoardPosition target;
    public readonly PieceType promotion;

    public Move(BoardPosition source, BoardPosition target, PieceType promotion)
    {
        this.source = source;
        this.target = target;
        this.promotion = promotion;
    }

    public Move(ReversibleMove move)
    {
        source = move.source;
        target = move.target;
        promotion = move.promotion;
    }

    public bool Equals(Move otherMove)
    {
        return Equals(source, otherMove.source) && Equals(target, otherMove.target) && Equals(promotion, otherMove.promotion);
    }

    public override string ToString()
    {
        var promotionText = "";
        switch (promotion)
        {
            case PieceType.WhiteRook:
                promotionText = "R";
                break;
            case PieceType.BlackRook:
                promotionText = "r";
                break;
            case PieceType.WhiteKnight:
                promotionText = "N";
                break;
            case PieceType.BlackKnight:
                promotionText = "n";
                break;
            case PieceType.WhiteBishop:
                promotionText = "B";
                break;
            case PieceType.BlackBishop:
                promotionText = "b";
                break;
            case PieceType.WhiteQueen:
                promotionText = "Q";
                break;
            case PieceType.BlackQueen:
                promotionText = "q";
                break;
        }

        return $"{source.pretty}{target.pretty}{promotionText}";
    }
}
