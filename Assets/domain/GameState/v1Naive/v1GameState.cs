using System.Collections.Generic;
using System.Linq;

public class V1GameState : GameStateInterface
{
    public int turn { get; private set; }
    public bool whiteTurn { get => turn % 2 == 0; }
    public int staleTurns { get; private set; }
    public BoardStateInterface BoardState { get => boardState; }
    public V1BoardState boardState { get; private set; }
    public List<ReversibleMove> history { get; }
    public Dictionary<BoardStateInterface, ushort> Snapshots { get => snapshots.ToDictionary(tuple => (BoardStateInterface)tuple.Key, tuple => tuple.Value); }
    public Dictionary<V1BoardState, ushort> snapshots { get; }

    public V1GameState()
    {
        turn = 0;
        staleTurns = 0;
        history = new List<ReversibleMove>();
        boardState = new V1BoardState();
        snapshots = new Dictionary<V1BoardState, ushort>();
    }

    public V1GameState(GameStateInterface gameState)
    {
        turn = gameState.turn;
        staleTurns = gameState.staleTurns;
        history = new List<ReversibleMove>(gameState.history);
        boardState = new V1BoardState(gameState.BoardState);
        snapshots = gameState.Snapshots.ToDictionary(tuple => new V1BoardState(tuple.Key), tuple => tuple.Value);
    }

    public List<Move> getLegalMoves()
    {
        return this.GenerateLegalMoves();
    }

    public void PlayMove(Move move)
    {
        var oldBoardState = boardState;
        var nextBoardPlay = oldBoardState.PlayMove(move);

        snapshots[oldBoardState] = (ushort)(snapshots.GetValueOrDefault(oldBoardState) + 1);
        boardState = nextBoardPlay.boardState;

        var lostWhiteKingCastleRight = oldBoardState.whiteCastleKingSide != nextBoardPlay.boardState.whiteCastleKingSide;
        var lostWhiteQueenCastleRight = oldBoardState.whiteCastleQueenSide != nextBoardPlay.boardState.whiteCastleQueenSide;
        var lostBlackKingCastleRight = oldBoardState.blackCastleKingSide != nextBoardPlay.boardState.blackCastleKingSide;
        var lostBlackQueenCastleRight = oldBoardState.blackCastleQueenSide != nextBoardPlay.boardState.blackCastleQueenSide;

        history.Add(new ReversibleMove(move.source, move.target, staleTurns, nextBoardPlay.sourcePiece.pieceType.IsPawn() && move.promotion != PieceType.Nothing, lostWhiteKingCastleRight, lostWhiteQueenCastleRight, lostBlackKingCastleRight, lostBlackQueenCastleRight, oldBoardState.enPassantColumn, nextBoardPlay.killedPiece));

        staleTurns = nextBoardPlay.sourcePiece.pieceType.IsPawn() || nextBoardPlay.killedPiece != null ? 0 : staleTurns + 1;

        ++turn;
    }

    public void UndoMove()
    {
        var reversibleMove = history[history.Count - 1];
        history.RemoveAt(history.Count - 1);
        boardState = boardState.UndoMove(reversibleMove);
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
        --turn;
    }

    public GameEndState GetGameEndState()
    {
        if (staleTurns >= 100) return GameEndState.Draw;
        if (snapshots.GetValueOrDefault(boardState) >= 2) return GameEndState.Draw;
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }
}
