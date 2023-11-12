
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class GameState
{
    public int turn { get; set; }
    public bool whiteTurn { get => turn % 2 == 0; }
    private List<PiecePosition> piecePositions;

    public GameState()
    {
        turn = 0;
        piecePositions = new List<PiecePosition> {
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

    public List<Move> getLegalMoves()
    {
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
                        new Move { source = pawnPosition.position, target = new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment) }   ,
                        new Move { source = pawnPosition.position, target = new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment * 2) }
                    };
                }

                if (pawnPosition.position.row == stopCondition - increment)
                {
                    return promotions.Select(promotion => new Move { source = pawnPosition.position, target = new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment), promotion = promotion }).ToList();
                }

                return new List<Move> { new Move { source = pawnPosition.position, target = new BoardPosition(pawnPosition.position.col, pawnPosition.position.row + increment) } };
            })
            .ToList();
    }

    public List<PiecePosition> getPiecePositions()
    {
        return piecePositions;
    }

    public PieceType getPieceAtPosition(BoardPosition position)
    {
        var piecePosition = piecePositions.Find(piece => piece.position.Equals(position));
        if (piecePosition == null) return PieceType.Nothing;
        return piecePosition.pieceType;
    }

    public void PlayMove(Move move)
    {
        piecePositions = piecePositions.Where(piece => !piece.position.Equals(move.target)).ToList();

        var sourcePiece = piecePositions.Find(piece => piece.position.Equals(move.source));
        if (sourcePiece == null) return;

        sourcePiece.position = move.target;

        ++turn;
    }

    public bool MoveResultsInPromotion(Move move)
    {

        var sourcePiece = piecePositions.Find(piece => piece.position.Equals(move.source));
        if (sourcePiece.pieceType != PieceType.BlackPawn && sourcePiece.pieceType != PieceType.WhitePawn) return false;

        if (sourcePiece.pieceType == PieceType.BlackPawn && move.target.row != 0) return false;
        if (sourcePiece.pieceType == PieceType.WhitePawn && move.target.row != 7) return false;

        return true;
    }
}
