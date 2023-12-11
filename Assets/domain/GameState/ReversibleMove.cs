public class ReversibleMove
{
    public BoardPosition source { get; }
    public BoardPosition target { get; }
    public int oldStaleTurns { get; }
    public PieceType promotion { get; }
    public bool lostWhiteKingCastleRight { get; }
    public bool lostWhiteQueenCastleRight { get; }
    public bool lostBlackKingCastleRight { get; }
    public bool lostBlackQueenCastleRight { get; }
    public int oldEnPassantColumn { get; }
    public PiecePosition killed { get; }

    public ReversibleMove(
        BoardPosition source,
        BoardPosition target,
        int oldStaleTurns,
        PieceType promotion,
        bool lostWhiteKingCastleRight,
        bool lostWhiteQueenCastleRight,
        bool lostBlackKingCastleRight,
        bool lostBlackQueenCastleRight,
        int oldEnPassantColumn,
        PiecePosition killed
    )
    {
        this.source = source;
        this.target = target;
        this.oldStaleTurns = oldStaleTurns;
        this.promotion = promotion;
        this.lostWhiteKingCastleRight = lostWhiteKingCastleRight;
        this.lostWhiteQueenCastleRight = lostWhiteQueenCastleRight;
        this.lostBlackKingCastleRight = lostBlackKingCastleRight;
        this.lostBlackQueenCastleRight = lostBlackQueenCastleRight;
        this.oldEnPassantColumn = oldEnPassantColumn;
        this.killed = killed;
    }
}
