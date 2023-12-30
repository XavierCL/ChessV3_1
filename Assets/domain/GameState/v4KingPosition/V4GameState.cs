using System.Collections.Generic;
using System.Linq;

public class V4GameState : GameStateInterface
{
    private int staleTurns;
    public override int StaleTurns { get => staleTurns; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V4BoardState boardState { get; private set; }
    public override List<ReversibleMove> history { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V4BoardState, ushort> snapshots { get; }

    public V4GameState()
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V4BoardState();
        snapshots = new Dictionary<V4BoardState, ushort>();
    }

    public V4GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.StaleTurns;
        history = new List<ReversibleMove>(gameState.history);
        boardState = new V4BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V4BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V4GameState(List<PiecePosition> piecePositions, bool whiteStarts, CastleFlags castling)
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V4BoardState(
            whiteStarts,
            piecePositions,
            castling
        );
        snapshots = new Dictionary<V4BoardState, ushort>();
    }

    public override List<Move> getLegalMoves()
    {
        return this.GenerateLegalMoves();
    }

    public override ReversibleMove PlayMove(Move move)
    {
        var oldBoardState = boardState;
        var nextBoardPlay = oldBoardState.PlayMove(move);

        snapshots[oldBoardState] = (ushort)(snapshots.GetValueOrDefault(oldBoardState) + 1);
        boardState = nextBoardPlay.boardState;

        var lostCastleRights = oldBoardState.castleFlags & ~nextBoardPlay.boardState.castleFlags;

        var reversibleMove = new ReversibleMove(
            move.source,
            move.target,
            StaleTurns,
            move.promotion,
            lostCastleRights,
            oldBoardState.enPassantColumn,
            nextBoardPlay.killedPiece
        );

        history.Add(reversibleMove);

        staleTurns = nextBoardPlay.sourcePiece.pieceType.IsPawn() || nextBoardPlay.killedPiece != null ? 0 : StaleTurns + 1;
        return reversibleMove;
    }

    public override void UndoMove()
    {
        var reversibleMove = history[^1];
        history.RemoveAt(history.Count - 1);
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
        var canOwnKingDie = V4LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
