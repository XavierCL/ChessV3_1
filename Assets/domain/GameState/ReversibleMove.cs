public class ReversibleMove
{
    public readonly BoardPosition source;
    public readonly BoardPosition target;
    public readonly int oldStaleTurns;
    public readonly PieceType promotion;
    public readonly CastleFlags lostCastleRights;
    public readonly int oldEnPassantColumn;
    public readonly PiecePosition killed;

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
