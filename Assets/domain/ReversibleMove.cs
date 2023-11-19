public class ReversibleMove
{
    public BoardPosition source { get; }
    public BoardPosition target { get; }
    public PieceType promotion { get; }
    public bool lostWhiteKingCastleRight { get; }
    public bool lostWhiteQueenCastleRight { get; }
    public bool lostBlackKingCastleRight { get; }
    public bool lostBlackQueenCastleRight { get; }
    public PiecePosition killed { get; }

    public ReversibleMove(BoardPosition source, BoardPosition target, PieceType promotion, bool lostBlackKingCastleRight, bool lostBlackQueenCastleRight, bool lostWhiteKingCastleRight, bool lostWhiteQueenCastleRight, PiecePosition killed)
    {
        this.source = source;
        this.target = target;
        this.promotion = promotion;
        this.lostWhiteKingCastleRight = lostWhiteKingCastleRight;
        this.lostWhiteQueenCastleRight = lostWhiteQueenCastleRight;
        this.lostBlackKingCastleRight = lostBlackKingCastleRight;
        this.lostBlackQueenCastleRight = lostBlackQueenCastleRight;
        this.killed = killed;
    }
}
