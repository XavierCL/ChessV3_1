using System.Collections.Generic;
using System.Linq;

// This game state drops support for piece position id. Don't use this in the UI.
public class V17GameState : GameStateInterface
{
    public int staleTurns;
    public override int StaleTurns { get => staleTurns; }
    public override BoardStateInterface BoardState { get => boardState; }
    public readonly V17BoardState boardState;
    public override List<ReversibleMove> History { get => history; }
    public List<ReversibleMove> history;
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V17BoardState.Hashable, ushort> snapshots;
    private IReadOnlyList<Move> legalMoves = null;
    private GameEndState endState = GameEndState.Nothing;

    public V17GameState()
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V17BoardState();
        snapshots = new Dictionary<V17BoardState.Hashable, ushort>();
    }

    public V17GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.StaleTurns;
        history = new List<ReversibleMove>(gameState.History);
        boardState = new V17BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V17BoardState.Hashable(new V17BoardState(tuple.Key)), tuple => tuple.Value);
    }

    public V17GameState(List<PiecePosition> piecePositions, bool whiteStarts, CastleFlags castling)
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V17BoardState(
            whiteStarts,
            piecePositions,
            castling
        );
        snapshots = new Dictionary<V17BoardState.Hashable, ushort>();
    }

    public override IReadOnlyList<Move> getLegalMoves()
    {
        if (legalMoves != null) return legalMoves;
        if (this.IsGameStateDraw()) legalMoves = V17LegalMoveGenerator.emptyMoveArray;
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
        var oldBoardHashable = boardState.GetHashable();
        var oldCastleFlags = boardState.castleFlags;
        var oldEnPassant = boardState.enPassantColumn;
        var boardPlay = boardState.PlayMove(move);

        snapshots.TryGetValue(oldBoardHashable, out var oldSnapshotCount);
        snapshots[oldBoardHashable] = (ushort)(oldSnapshotCount + 1);

        var lostCastleRights = oldCastleFlags & ~boardState.castleFlags;

        var reversibleMove = new ReversibleMove(
            move.source,
            move.target,
            staleTurns,
            move.promotion,
            lostCastleRights,
            oldEnPassant,
            boardPlay.killedPiece
        );

        history.Add(reversibleMove);

        staleTurns = boardPlay.sourcePieceType.IsPawn() || boardPlay.killedPiece != null ? 0 : staleTurns + 1;
        legalMoves = null;
        endState = GameEndState.Nothing;
        return reversibleMove;
    }

    public override void UndoMove()
    {
        var reversibleMove = history[^1];
        history.RemoveAt(history.Count - 1);
        boardState.UndoMove(reversibleMove);
        var undoneBoardStateHashable = boardState.GetHashable();

        if (!snapshots.TryGetValue(undoneBoardStateHashable, out var oldSnapshotCount))
        {
            throw new System.Exception("Could not undo move: board isn't a snapshot");
        }

        if (oldSnapshotCount == 1)
        {
            snapshots.Remove(undoneBoardStateHashable);
        }
        else
        {
            snapshots[undoneBoardStateHashable] -= 1;
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

    private GameEndState GenerateGameEndState()
    {
        if (legalMoves != null ? legalMoves.Count > 0 : V17LegalMoveGenerator.GenerateHasLegalMoves(this)) return GameEndState.Ongoing;
        var canOwnKingDie = V17LegalMoveGenerator.CanOwnKingDie(this);

        // Stalemate
        if (!canOwnKingDie) return GameEndState.Draw;

        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
