using System.Collections.Generic;
using System.Linq;

public class V3GameState : GameStateInterface
{
    public override int staleTurns { get; protected set; }
    public override BoardStateInterface BoardState { get => boardState; }
    public V3BoardState boardState { get; private set; }
    public override List<ReversibleMove> history { get; }
    public override Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V3BoardState, ushort> snapshots { get; }

    public V3GameState()
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V3BoardState();
        snapshots = new Dictionary<V3BoardState, ushort>();
    }

    public V3GameState(GameStateInterface gameState)
    {
        staleTurns = gameState.staleTurns;
        history = new List<ReversibleMove>(gameState.history);
        boardState = new V3BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V3BoardState(tuple.Key), tuple => tuple.Value);
    }

    public V3GameState(List<PiecePosition> piecePositions, bool whiteStarts, Castling castling)
    {
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V3BoardState(
            whiteStarts,
            piecePositions,
            (castling & Castling.WhiteKing) == Castling.WhiteKing,
            (castling & Castling.WhiteQueen) == Castling.WhiteQueen,
            (castling & Castling.BlackKing) == Castling.BlackKing,
            (castling & Castling.BlackQueen) == Castling.BlackQueen
        );
        snapshots = new Dictionary<V3BoardState, ushort>();
    }

    public override List<Move> getLegalMoves()
    {
        return this.GenerateLegalMoves();
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
            move.promotion,
            lostWhiteKingCastleRight,
            lostWhiteQueenCastleRight,
            lostBlackKingCastleRight,
            lostBlackQueenCastleRight,
            oldBoardState.enPassantColumn,
            nextBoardPlay.killedPiece
        ));

        staleTurns = nextBoardPlay.sourcePiece.pieceType.IsPawn() || nextBoardPlay.killedPiece != null ? 0 : staleTurns + 1;
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
        if (staleTurns >= 100) return GameEndState.Draw;
        if (snapshots.GetValueOrDefault(boardState) >= 2) return GameEndState.Draw;
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = V3LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return boardState.whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
