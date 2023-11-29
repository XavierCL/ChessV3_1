using System.Collections.Generic;
using System.Linq;

public class V5GameState : GameStateInterface
{
    public override int turn { get; protected set; }
    public override int staleTurns { get; protected set; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V5BoardState boardState { get; private set; }
    public override List<ReversibleMove> history { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V5BoardState, ushort> snapshots { get; }
    private List<Move> legalMoves { get; set; } = null;

    public V5GameState()
    {
        turn = 0;
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V5BoardState();
        snapshots = new Dictionary<V5BoardState, ushort>();
    }

    public V5GameState(GameStateInterface gameState)
    {
        turn = gameState.turn;
        staleTurns = gameState.staleTurns;
        history = new List<ReversibleMove>(gameState.history);
        boardState = new V5BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V5BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V5GameState(List<PiecePosition> piecePositions, bool whiteStarts, Castling castling)
    {
        turn = whiteStarts ? 0 : 1;
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V5BoardState(
            piecePositions,
            (castling & Castling.WhiteKing) == Castling.WhiteKing,
            (castling & Castling.WhiteQueen) == Castling.WhiteQueen,
            (castling & Castling.BlackKing) == Castling.BlackKing,
            (castling & Castling.BlackQueen) == Castling.BlackQueen
        );
        snapshots = new Dictionary<V5BoardState, ushort>();
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

        ++turn;
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
        --turn;
    }

    public override GameEndState GetGameEndState()
    {
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = V5LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
