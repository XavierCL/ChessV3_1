
using System.Collections.Generic;
using System.Linq;

public class GameState
{
    public int turn { get; set; }
    public bool whiteTurn { get => turn % 2 == 0; }
    public List<PiecePosition> piecePositions { get; private set; }

    public GameState()
    {
        turn = 0;
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

    public GameState(GameState gameState)
    {
        turn = gameState.turn;
        piecePositions = gameState.piecePositions.Select(piecePosition => new PiecePosition(piecePosition)).ToList();
    }

    public List<Move> getLegalMoves()
    {
        return this.GenerateLegalMoves();
        var ownPawn = whiteTurn ? PieceType.WhitePawn : PieceType.BlackPawn;
        var ownPawnStartingY = whiteTurn ? 1 : 6;
        var increment = whiteTurn ? 1 : -1;
        var stopCondition = whiteTurn ? 7 : 0;
        var promotions = whiteTurn
            ? new List<PieceType> { PieceType.WhiteRook, PieceType.WhiteKnight, PieceType.WhiteBishop, PieceType.WhiteQueen }
            : new List<PieceType> { PieceType.BlackRook, PieceType.BlackKnight, PieceType.BlackBishop, PieceType.BlackQueen };

        return piecePositions
            .Where(piecePosition => piecePosition.pieceType == ownPawn && piecePosition.position.row != stopCondition)
            .SelectMany(pawnPosition =>
            {
                if (pawnPosition.position.row == ownPawnStartingY)
                {
                    return new List<Move> {
                        new Move(pawnPosition.position, new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment), PieceType.Nothing)   ,
                        new Move(pawnPosition.position, new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment * 2),   PieceType.Nothing)
                    };
                }

                if (pawnPosition.position.row == stopCondition - increment)
                {
                    return promotions.Select(promotion => new Move(pawnPosition.position, new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment), promotion)).ToList();
                }

                return new List<Move> { new Move(pawnPosition.position, new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment), PieceType.Nothing) };
            })
            .ToList();
    }

    public void PlayMove(Move move)
    {
        piecePositions = piecePositions.Where(piece => !piece.position.Equals(move.target)).ToList();

        var sourcePiece = piecePositions.Find(piece => piece.position.Equals(move.source));
        if (sourcePiece == null) return;

        sourcePiece.position = move.target;

        if (move.promotion != PieceType.Nothing)
        {
            sourcePiece.pieceType = move.promotion;
        }

        ++turn;
    }

    public GameEndState GetGameEndState()
    {
        if (getLegalMoves().Count == 0) return GameEndState.Draw;

        return GameEndState.Ongoing;
    }
}
