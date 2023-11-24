public class ReversibleMove
{
    public BoardPosition source { get; }
    public BoardPosition target { get; }
    public bool pawnPromoted { get; }
    public bool lostWhiteKingCastleRight { get; }
    public bool lostWhiteQueenCastleRight { get; }
    public bool lostBlackKingCastleRight { get; }
    public bool lostBlackQueenCastleRight { get; }
    public PiecePosition killed { get; }

    public ReversibleMove(BoardPosition source, BoardPosition target, bool pawnPromoted, bool lostBlackKingCastleRight, bool lostBlackQueenCastleRight, bool lostWhiteKingCastleRight, bool lostWhiteQueenCastleRight, PiecePosition killed)
    {
        this.source = source;
        this.target = target;
        this.pawnPromoted = pawnPromoted;
        this.lostWhiteKingCastleRight = lostWhiteKingCastleRight;
        this.lostWhiteQueenCastleRight = lostWhiteQueenCastleRight;
        this.lostBlackKingCastleRight = lostBlackKingCastleRight;
        this.lostBlackQueenCastleRight = lostBlackQueenCastleRight;
        this.killed = killed;
    }
}
