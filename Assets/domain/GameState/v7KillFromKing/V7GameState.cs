using System.Collections.Generic;
using System.Linq;

// This game state drops support for piece position id. Don't use this in the UI.
public class V7GameState : GameStateInterface
{
    public override int staleTurns { get; protected set; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V7BoardState boardState { get; private set; }
    public override List<ReversibleMove> history { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V7BoardState, ushort> snapshots { get; }
    private List<Move> legalMoves { get; set; } = null;

    public V7GameState()
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V7BoardState();
        snapshots = new Dictionary<V7BoardState, ushort>();
    }

    public V7GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.staleTurns;
        history = new List<ReversibleMove>(gameState.history);
        boardState = new V7BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V7BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V7GameState(List<PiecePosition> piecePositions, bool whiteStarts, Castling castling)
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V7BoardState(
            whiteStarts,
            piecePositions,
            (castling & Castling.WhiteKing) == Castling.WhiteKing,
            (castling & Castling.WhiteQueen) == Castling.WhiteQueen,
            (castling & Castling.BlackKing) == Castling.BlackKing,
            (castling & Castling.BlackQueen) == Castling.BlackQueen
        );
        snapshots = new Dictionary<V7BoardState, ushort>();
    }

    public override List<Move> getLegalMoves()
    {
        if (legalMoves != null) return legalMoves;
        legalMoves = this.GenerateLegalMoves();
        return legalMoves;
    }

    public override void PlayMove(Move move)
    {
        var oldBoardState = boardState;
        var nextBoardPlay = oldBoardState.PlayMove(move);

        snapshots[oldBoardState] = (ushort)(snapshots.GetValueOrDefault(oldBoardState) + 1);
        boardState = nextBoardPlay.boardState;

        var lostWhiteKingCastleRight = oldBoardState.whiteCastleKingSide != nextBoardPlay.boardState.whiteCastleKingSide;
        var lostWhiteQueenCastleRight = oldBoardState.whiteCastleQueenSide != nextBoardPlay.boardState.whiteCastleQueenSide;
        var lostBlackKingCastleRight = oldBoardState.blackCastleKingSide != nextBoardPlay.boardState.blackCastleKingSide;
        var lostBlackQueenCastleRight = oldBoardState.blackCastleQueenSide != nextBoardPlay.boardState.blackCastleQueenSide;

        history.Add(new ReversibleMove(
            move.source,
            move.target,
            staleTurns,
            nextBoardPlay.sourcePiece.pieceType.IsPawn() && move.promotion != PieceType.Nothing,
            lostWhiteKingCastleRight,
            lostWhiteQueenCastleRight,
            lostBlackKingCastleRight,
            lostBlackQueenCastleRight,
            oldBoardState.enPassantColumn,
            nextBoardPlay.killedPiece
        ));

        staleTurns = nextBoardPlay.sourcePiece.pieceType.IsPawn() || nextBoardPlay.killedPiece != null ? 0 : staleTurns + 1;
        legalMoves = null;
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
        legalMoves = null;
    }

    public override GameEndState GetGameEndState()
    {
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = V7LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
