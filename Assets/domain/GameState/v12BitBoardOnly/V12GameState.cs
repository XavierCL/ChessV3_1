using System.Collections.Generic;
using System.Linq;

// This game state drops support for piece position id. Don't use this in the UI.
public class V12GameState : GameStateInterface
{
    private int staleTurns;
    public override int StaleTurns { get => staleTurns; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V12BoardState boardState;
    public override List<ReversibleMove> History { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V12BoardState, ushort> snapshots { get; }
    private List<Move> legalMoves = null;

    public V12GameState()
    {
        staleTurns = 0;
        History = new List<ReversibleMove>();
        boardState = new V12BoardState();
        snapshots = new Dictionary<V12BoardState, ushort>();
    }

    public V12GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.StaleTurns;
        History = new List<ReversibleMove>(gameState.History);
        boardState = new V12BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V12BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V12GameState(List<PiecePosition> piecePositions, bool whiteStarts, CastleFlags castling)
    {
        staleTurns = 0;
        History = new List<ReversibleMove>();
        boardState = new V12BoardState(
            whiteStarts,
            piecePositions,
            castling
        );
        snapshots = new Dictionary<V12BoardState, ushort>();
    }

    public override IReadOnlyList<Move> getLegalMoves()
    {
        if (legalMoves != null) return legalMoves;
        legalMoves = this.GenerateLegalMoves();
        return legalMoves;
    }

    public override ReversibleMove PlayMove(Move move)
    {
        var oldBoardState = boardState;
        var nextBoardPlay = oldBoardState.PlayMove(move);

        snapshots[oldBoardState] = (ushort)(snapshots.GetValueOrDefault(oldBoardState) + 1);
        boardState = nextBoardPlay.boardState;

        var lostCastleRights = oldBoardState.CastleFlags & ~nextBoardPlay.boardState.CastleFlags;

        var reversibleMove = new ReversibleMove(
            move.source,
            move.target,
            StaleTurns,
            move.promotion,
            lostCastleRights,
            oldBoardState.EnPassantColumn,
            nextBoardPlay.killedPiece
        );

        History.Add(reversibleMove);

        staleTurns = nextBoardPlay.sourcePiece.pieceType.IsPawn() || nextBoardPlay.killedPiece != null ? 0 : StaleTurns + 1;
        legalMoves = null;
        return reversibleMove;
    }

    public override void UndoMove()
    {
        var reversibleMove = History[^1];
        History.RemoveAt(History.Count - 1);
        boardState = boardState.UndoMove(reversibleMove);
        if (!snapshots.ContainsKey(boardState))
        {
            throw new System.Exception("Could not undo move: board isn't a snapshot");
        }
        var oldSnapshotCount = snapshots[boardState];

        if (oldSnapshotCount == 1)
        {
            snapshots.Remove(boardState);
        }
        else
        {
            snapshots[boardState] -= 1;
        }

        staleTurns = reversibleMove.oldStaleTurns;
        legalMoves = null;
    }

    public override GameEndState GetGameEndState()
    {
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = V12LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
