
using System.Collections.Generic;
using System.Linq;

public class V1GameState : GameStateInterface
{
    public int turn { get; private set; }
    public bool whiteTurn { get => turn % 2 == 0; }
    public List<PiecePosition> piecePositions { get; }
    public List<ReversibleMove> history { get; }

    public V1GameState()
    {
        turn = 0;
        history = new List<ReversibleMove>();
        piecePositions = new List<PiecePosition> {
            new PiecePosition("a1", PieceType.WhiteRook, new BoardPosition(0, 0)),
            new PiecePosition("b1", PieceType.WhiteKnight, new BoardPosition(1, 0)),
            new PiecePosition("c1", PieceType.WhiteBishop, new BoardPosition(2, 0)),
            new PiecePosition("d1", PieceType.WhiteQueen, new BoardPosition(3, 0)),
            new PiecePosition("e1", PieceType.WhiteKing, new BoardPosition(4, 0)),
            new PiecePosition("f1", PieceType.WhiteBishop, new BoardPosition(5, 0)),
            new PiecePosition("g1", PieceType.WhiteKnight, new BoardPosition(6, 0)),
            new PiecePosition("h1", PieceType.WhiteRook, new BoardPosition(7, 0)),
            new PiecePosition("a2", PieceType.WhitePawn, new BoardPosition(0, 1)),
            new PiecePosition("b2", PieceType.WhitePawn, new BoardPosition(1, 1)),
            new PiecePosition("c2", PieceType.WhitePawn, new BoardPosition(2, 1)),
            new PiecePosition("d2", PieceType.WhitePawn, new BoardPosition(3, 1)),
            new PiecePosition("e2", PieceType.WhitePawn, new BoardPosition(4, 1)),
            new PiecePosition("f2", PieceType.WhitePawn, new BoardPosition(5, 1)),
            new PiecePosition("g2", PieceType.WhitePawn, new BoardPosition(6, 1)),
            new PiecePosition("h2", PieceType.WhitePawn, new BoardPosition(7, 1)),
            new PiecePosition("a7", PieceType.BlackRook, new BoardPosition(0, 7)),
            new PiecePosition("b7", PieceType.BlackKnight, new BoardPosition(1, 7)),
            new PiecePosition("c7", PieceType.BlackBishop, new BoardPosition(2, 7)),
            new PiecePosition("d7", PieceType.BlackQueen, new BoardPosition(3, 7)),
            new PiecePosition("e7", PieceType.BlackKing, new BoardPosition(4, 7)),
            new PiecePosition("f7", PieceType.BlackBishop, new BoardPosition(5, 7)),
            new PiecePosition("g7", PieceType.BlackKnight, new BoardPosition(6, 7)),
            new PiecePosition("h7", PieceType.BlackRook, new BoardPosition(7, 7)),
            new PiecePosition("a6", PieceType.BlackPawn, new BoardPosition(0, 6)),
            new PiecePosition("b6", PieceType.BlackPawn, new BoardPosition(1, 6)),
            new PiecePosition("c6", PieceType.BlackPawn, new BoardPosition(2, 6)),
            new PiecePosition("d6", PieceType.BlackPawn, new BoardPosition(3, 6)),
            new PiecePosition("e6", PieceType.BlackPawn, new BoardPosition(4, 6)),
            new PiecePosition("f6", PieceType.BlackPawn, new BoardPosition(5, 6)),
            new PiecePosition("g6", PieceType.BlackPawn, new BoardPosition(6, 6)),
            new PiecePosition("h6", PieceType.BlackPawn, new BoardPosition(7, 6)),
        };
    }

    public V1GameState(GameStateInterface gameState)
    {
        turn = gameState.turn;
        history = new List<ReversibleMove>(gameState.history);
        piecePositions = new List<PiecePosition>(gameState.piecePositions);
    }

    public List<Move> getLegalMoves()
    {
        return this.GenerateLegalMoves();
    }

    public void PlayMove(Move move)
    {
        var killedPieceIndex = piecePositions.FindIndex(piece => piece.position.Equals(move.target));
        PiecePosition killedPiece = null;

        if (killedPieceIndex != -1)
        {
            killedPiece = piecePositions[killedPieceIndex];
            piecePositions.RemoveAt(killedPieceIndex);
        }

        var sourcePieceIndex = piecePositions.FindIndex(piece => piece.position.Equals(move.source));

        if (sourcePieceIndex == -1)
        {
            throw new System.Exception("Invalid move, no piece at source position");
        }

        var sourcePiece = piecePositions[sourcePieceIndex];

        piecePositions[sourcePieceIndex] = piecePositions[sourcePieceIndex].PlayMove(move.target, move.promotion);

        // En passant
        if (sourcePiece.pieceType.IsPawn() && move.source.col != move.target.col && killedPieceIndex == -1)
        {
            killedPieceIndex = piecePositions.FindIndex(piece => piece.position.Equals(new BoardPosition(move.target.col, move.source.row)));

            if (killedPieceIndex == -1)
            {
                throw new System.Exception("Invalid move, en passant without enemy piece");
            }

            killedPiece = piecePositions[killedPieceIndex];
            piecePositions.RemoveAt(killedPieceIndex);
        }

        history.Add(new ReversibleMove(move.source, move.target, sourcePiece.pieceType.IsPawn() && move.promotion != PieceType.Nothing, false, false, false, false, killedPiece));

        ++turn;
    }

    public void UndoMove()
    {
        var reversibleMove = history[history.Count - 1];
        history.RemoveAt(history.Count - 1);

        var sourcePieceIndex = piecePositions.FindIndex(piece => piece.position.Equals(reversibleMove.target));

        if (sourcePieceIndex == -1)
        {
            history.Add(reversibleMove);
            throw new System.Exception("Invalid undo, no piece at target position");
        }

        var sourcePiece = piecePositions[sourcePieceIndex];
        piecePositions[sourcePieceIndex] = sourcePiece.PlayMove(reversibleMove.source, reversibleMove.pawnPromoted ? sourcePiece.pieceType.IsWhite() ? PieceType.WhitePawn : PieceType.BlackPawn : PieceType.Nothing);

        if (reversibleMove.killed != null)
        {
            piecePositions.Add(reversibleMove.killed);
        }

        --turn;
    }

    public GameEndState GetGameEndState()
    {
        if (getLegalMoves().Count > 0) return GameEndState.Ongoing;
        var canOwnKingDie = LegalMoveGenerator.CanOwnKingDie(this);
        if (!canOwnKingDie) return GameEndState.Draw;
        return whiteTurn ? GameEndState.BlackWin : GameEndState.WhiteWin;
    }

    public bool HasKing(bool white)
    {
        return piecePositions.Any(piecePosition => white ? piecePosition.pieceType == PieceType.WhiteKing : piecePosition.pieceType == PieceType.BlackKing);
    }
}
