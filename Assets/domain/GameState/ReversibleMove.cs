public class ReversibleMove
{
    public BoardPosition source { get; }
    public BoardPosition target { get; }
    public int oldStaleTurns { get; }
    public bool pawnPromoted { get; }
    public bool lostWhiteKingCastleRight { get; }
    public bool lostWhiteQueenCastleRight { get; }
    public bool lostBlackKingCastleRight { get; }
    public bool lostBlackQueenCastleRight { get; }
    public int oldPassantColumn { get; }
    public PiecePosition killed { get; }

    public ReversibleMove(
        BoardPosition source,
        BoardPosition target,
        int oldStaleTurns,
        bool pawnPromoted,
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
        this.pawnPromoted = pawnPromoted;
        this.lostWhiteKingCastleRight = lostWhiteKingCastleRight;
        this.lostWhiteQueenCastleRight = lostWhiteQueenCastleRight;
        this.lostBlackKingCastleRight = lostBlackKingCastleRight;
        this.lostBlackQueenCastleRight = lostBlackQueenCastleRight;
        this.oldPassantColumn = oldEnPassantColumn;
        this.killed = killed;
    }
}
