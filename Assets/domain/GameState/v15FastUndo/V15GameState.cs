using System.Collections.Generic;
using System.Linq;

// This game state drops support for piece position id. Don't use this in the UI.
public class V15GameState : GameStateInterface
{
    public int staleTurns;
    public override int StaleTurns { get => staleTurns; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V15BoardState boardState;
    public override List<ReversibleMove> History { get => history; }
    public List<ReversibleMove> history;
    public readonly List<V15BoardState> boardHistory;
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V15BoardState, ushort> snapshots { get; }
    private IReadOnlyList<Move> legalMoves = null;
    private GameEndState endState = GameEndState.Nothing;

    public V15GameState()
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V15BoardState();
        snapshots = new Dictionary<V15BoardState, ushort>();
        boardHistory = new List<V15BoardState>();
    }

    public V15GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.StaleTurns;
        history = new List<ReversibleMove>(gameState.History);
        boardState = new V15BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V15BoardState(tuple.Key), tuple => tuple.Value);

        boardHistory = new List<V15BoardState>(history.Count);
        var lastBoardState = boardState;

        for (var moveIndex = history.Count - 1; moveIndex >= 0; --moveIndex)
        {
            lastBoardState = boardState.UndoMove(history[moveIndex]);
            boardHistory.Add(lastBoardState);
        }

        boardHistory.Reverse();
    }

    public V15GameState(List<PiecePosition> piecePositions, bool whiteStarts, CastleFlags castling)
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V15BoardState(
            whiteStarts,
            piecePositions,
            castling
        );
        snapshots = new Dictionary<V15BoardState, ushort>();
    }

    public override IReadOnlyList<Move> getLegalMoves()
    {
        if (legalMoves != null) return legalMoves;
        if (this.IsGameStateDraw()) legalMoves = V15LegalMoveGenerator.emptyMoveArray;
        else legalMoves = this.GenerateLegalMoves();
        return legalMoves;
    }

    public IReadOnlyList<Move> getLegalMovesWithoutGameStateCheck()
    {
        if (legalMoves != null) return legalMoves;
        legalMoves = this.GenerateLegalMoves();
        return legalMoves;
    }

    public override ReversibleMove PlayMove(Move move)
    {
        var oldBoardState = boardState;
        var nextBoardPlay = oldBoardState.PlayMove(move);

        snapshots.TryGetValue(oldBoardState, out var oldSnapshotCount);
        snapshots[oldBoardState] = (ushort)(oldSnapshotCount + 1);
        boardState = nextBoardPlay.boardState;

        var lostCastleRights = oldBoardState.castleFlags & ~nextBoardPlay.boardState.castleFlags;

        var reversibleMove = new ReversibleMove(
            move.source,
            move.target,
            staleTurns,
            move.promotion,
            lostCastleRights,
            oldBoardState.enPassantColumn,
            nextBoardPlay.killedPiece
        );

        history.Add(reversibleMove);
        boardHistory.Add(oldBoardState);

        staleTurns = nextBoardPlay.sourcePiece.pieceType.IsPawn() || nextBoardPlay.killedPiece != null ? 0 : staleTurns + 1;
        legalMoves = null;
        endState = GameEndState.Nothing;
        return reversibleMove;
    }

    public override void UndoMove()
    {
        var reversibleMove = history[^1];
        history.RemoveAt(history.Count - 1);
        boardState = boardHistory[^1];
        boardHistory.RemoveAt(boardHistory.Count - 1);

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
        if (this.IsGameStateDraw()) endState = GameEndState.Draw;
        else endState = GenerateGameEndState();
        return endState;
    }

    public GameEndState GetGameEndStateWithoutGameStateCheck()
    {
        if (endState != GameEndState.Nothing) return endState;
        endState = GenerateGameEndState();
        return endState;
    }

    private GameEndState GenerateGameEndState()
    {
        if (legalMoves != null ? legalMoves.Count > 0 : V15LegalMoveGenerator.GenerateHasLegalMoves(this)) return GameEndState.Ongoing;
        var canOwnKingDie = V15LegalMoveGenerator.CanOwnKingDie(this);

        // Stalemate
        if (!canOwnKingDie) return GameEndState.Draw;

        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
