using System.Collections.Generic;
using System.Linq;

public class V3GameState : GameStateInterface
{
    private int staleTurns;
    public override int StaleTurns { get => staleTurns; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V3BoardState boardState { get; private set; }
    public override List<ReversibleMove> History { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V3BoardState, ushort> snapshots { get; }

    public V3GameState()
    {
        staleTurns = 0;
        History = new List<ReversibleMove>();
        boardState = new V3BoardState();
        snapshots = new Dictionary<V3BoardState, ushort>();
    }

    public V3GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.StaleTurns;
        History = new List<ReversibleMove>(gameState.History);
        boardState = new V3BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V3BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V3GameState(List<PiecePosition> piecePositions, bool whiteStarts, CastleFlags castling)
    {
        staleTurns = 0;
        History = new List<ReversibleMove>();
        boardState = new V3BoardState(
            whiteStarts,
            piecePositions,
            castling
        );
        snapshots = new Dictionary<V3BoardState, ushort>();
    }

    public override IReadOnlyList<Move> getLegalMoves()
    {
        return this.GenerateLegalMoves();
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
    }

    public override GameEndState GetGameEndState()
    {
        if (StaleTurns >= 100) return GameEndState.Draw;
        if (snapshots.GetValueOrDefault(boardState) >= 2) return GameEndState.Draw;
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = V3LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return boardState.WhiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
