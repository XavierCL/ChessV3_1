public class ReversibleMove
{
    public BoardPosition source { get; }
    public BoardPosition target { get; }
    public int oldStaleTurns { get; }
    public PieceType promotion { get; }
    public CastleFlags lostCastleRights { get; }
    public int oldEnPassantColumn { get; }
    public PiecePosition killed { get; }

    public ReversibleMove(
        BoardPosition source,
        BoardPosition target,
        int oldStaleTurns,
        PieceType promotion,
        CastleFlags lostCastleRights,
        int oldEnPassantColumn,
        PiecePosition killed
    )
    {
        this.source = source;
        this.target = target;
        this.oldStaleTurns = oldStaleTurns;
        this.promotion = promotion;
        this.lostCastleRights = lostCastleRights;
        this.oldEnPassantColumn = oldEnPassantColumn;
        this.killed = killed;
    }
}
