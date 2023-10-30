
using System.Collections.Generic;

public class GameState
{
    public int turn { get; set; }
    public bool whiteTurn { get => turn % 2 == 0; }

    public GameState()
    {
        turn = 0;
    }

    public List<Move> getLegalMoves()
    {
        if (whiteTurn)
            return new List<Move> { new Move { source = new BoardPosition(0, 1), target = new BoardPosition(0, 2) } };
        else return new List<Move> { new Move { source = new BoardPosition(7, 6), target = new BoardPosition(7, 5) } };
    }

    public List<PiecePosition> getPiecePositions()
    {
        return new List<PiecePosition> {
            new PiecePosition{pieceType=PieceType.WhiteRook, position=new BoardPosition(0, 0)},
            new PiecePosition{pieceType=PieceType.WhiteKnight, position=new BoardPosition(1, 0)},
            new PiecePosition{pieceType=PieceType.WhiteBishop, position=new BoardPosition(2, 0)},
            new PiecePosition{pieceType=PieceType.WhiteQueen, position=new BoardPosition(3, 0)},
            new PiecePosition{pieceType=PieceType.WhiteKing, position=new BoardPosition(4, 0)},
            new PiecePosition{pieceType=PieceType.WhiteBishop, position=new BoardPosition(5, 0)},
            new PiecePosition{pieceType=PieceType.WhiteKnight, position=new BoardPosition(6, 0)},
            new PiecePosition{pieceType=PieceType.WhiteRook, position=new BoardPosition(7, 0)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(0, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(1, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(2, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(3, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(4, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(5, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(6, 1)},
            new PiecePosition{pieceType=PieceType.WhitePawn, position=new BoardPosition(7, 1)},
            new PiecePosition{pieceType=PieceType.BlackRook, position=new BoardPosition(0, 7)},
            new PiecePosition{pieceType=PieceType.BlackKnight, position=new BoardPosition(1, 7)},
            new PiecePosition{pieceType=PieceType.BlackBishop, position=new BoardPosition(2, 7)},
            new PiecePosition{pieceType=PieceType.BlackQueen, position=new BoardPosition(3, 7)},
            new PiecePosition{pieceType=PieceType.BlackKing, position=new BoardPosition(4, 7)},
            new PiecePosition{pieceType=PieceType.BlackBishop, position=new BoardPosition(5, 7)},
            new PiecePosition{pieceType=PieceType.BlackKnight, position=new BoardPosition(6, 7)},
            new PiecePosition{pieceType=PieceType.BlackRook, position=new BoardPosition(7, 7)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(0, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(1, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(2, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(3, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(4, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(5, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(6, 6)},
            new PiecePosition{pieceType=PieceType.BlackPawn, position=new BoardPosition(7, 6)},
        };
    }

    public void PlayMove(Move move)
    {
        ++turn;
    }
}
