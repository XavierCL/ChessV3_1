using System.Collections.Generic;
using System.Linq;

// This game state drops support for piece position id. Don't use this in the UI.
public class V14GameState : GameStateInterface
{
    private int staleTurns;
    public override int StaleTurns { get => staleTurns; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V14BoardState boardState;
    public override List<ReversibleMove> history { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V14BoardState, ushort> snapshots { get; }
    private IReadOnlyList<Move> legalMoves = null;
    private GameEndState endState = GameEndState.Nothing;

    public V14GameState()
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V14BoardState();
        snapshots = new Dictionary<V14BoardState, ushort>();
    }

    public V14GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.StaleTurns;
        history = new List<ReversibleMove>(gameState.history);
        boardState = new V14BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V14BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V14GameState(List<PiecePosition> piecePositions, bool whiteStarts, CastleFlags castling)
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V14BoardState(
            whiteStarts,
            piecePositions,
            castling
        );
        snapshots = new Dictionary<V14BoardState, ushort>();
    }

    public override IReadOnlyList<Move> getLegalMoves()
    {
        if (legalMoves != null) return legalMoves;
        if (this.IsGameStateDraw()) legalMoves = V14LegalMoveGenerator.emptyMoveArray;
        else legalMoves = this.GenerateLegalMoves();
        return legalMoves;
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
        legalMoves = null;
        endState = GameEndState.Nothing;
        return reversibleMove;
    }

    public override void UndoMove()
    {
        var reversibleMove = history[^1];
        history.RemoveAt(history.Count - 1);
        boardState = boardState.UndoMove(reversibleMove);

        if (!snapshots.TryGetValue(boardState, out var oldSnapshotCount))
        {
            throw new System.Exception("Could not undo move: board isn't a snapshot");
        }

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
        endState = GameEndState.Nothing;
    }

    public override GameEndState GetGameEndState()
    {
        if (endState != GameEndState.Nothing) return endState;
        endState = GenerateGameEndState();
        return endState;
    }

    private GameEndState GenerateGameEndState()
    {
        if (legalMoves != null ? legalMoves.Count > 0 : V14LegalMoveGenerator.GenerateHasLegalMoves(this)) return GameEndState.Ongoing;
        if (this.IsGameStateDraw()) return GameEndState.Draw;
        var canOwnKingDie = V14LegalMoveGenerator.CanOwnKingDie(this);

        // Stalemate
        if (!canOwnKingDie) return GameEndState.Draw;

        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
