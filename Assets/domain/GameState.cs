
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
            new PiecePosition{id="a1", pieceType=PieceType.WhiteRook, position=new BoardPosition(0, 0)},
            new PiecePosition{id="b1", pieceType=PieceType.WhiteKnight, position=new BoardPosition(1, 0)},
            new PiecePosition{id="c1", pieceType=PieceType.WhiteBishop, position=new BoardPosition(2, 0)},
            new PiecePosition{id="d1", pieceType=PieceType.WhiteQueen, position=new BoardPosition(3, 0)},
            new PiecePosition{id="e1", pieceType=PieceType.WhiteKing, position=new BoardPosition(4, 0)},
            new PiecePosition{id="f1", pieceType=PieceType.WhiteBishop, position=new BoardPosition(5, 0)},
            new PiecePosition{id="g1", pieceType=PieceType.WhiteKnight, position=new BoardPosition(6, 0)},
            new PiecePosition{id="h1", pieceType=PieceType.WhiteRook, position=new BoardPosition(7, 0)},
            new PiecePosition{id="a2", pieceType=PieceType.WhitePawn, position=new BoardPosition(0, 1)},
            new PiecePosition{id="b2", pieceType=PieceType.WhitePawn, position=new BoardPosition(1, 1)},
            new PiecePosition{id="c2", pieceType=PieceType.WhitePawn, position=new BoardPosition(2, 1)},
            new PiecePosition{id="d2", pieceType=PieceType.WhitePawn, position=new BoardPosition(3, 1)},
            new PiecePosition{id="e2", pieceType=PieceType.WhitePawn, position=new BoardPosition(4, 1)},
            new PiecePosition{id="f2", pieceType=PieceType.WhitePawn, position=new BoardPosition(5, 1)},
            new PiecePosition{id="g2", pieceType=PieceType.WhitePawn, position=new BoardPosition(6, 1)},
            new PiecePosition{id="h2", pieceType=PieceType.WhitePawn, position=new BoardPosition(7, 1)},
            new PiecePosition{id="a7", pieceType=PieceType.BlackRook, position=new BoardPosition(0, 7)},
            new PiecePosition{id="b7", pieceType=PieceType.BlackKnight, position=new BoardPosition(1, 7)},
            new PiecePosition{id="c7", pieceType=PieceType.BlackBishop, position=new BoardPosition(2, 7)},
            new PiecePosition{id="d7", pieceType=PieceType.BlackQueen, position=new BoardPosition(3, 7)},
            new PiecePosition{id="e7", pieceType=PieceType.BlackKing, position=new BoardPosition(4, 7)},
            new PiecePosition{id="f7", pieceType=PieceType.BlackBishop, position=new BoardPosition(5, 7)},
            new PiecePosition{id="g7", pieceType=PieceType.BlackKnight, position=new BoardPosition(6, 7)},
            new PiecePosition{id="h7", pieceType=PieceType.BlackRook, position=new BoardPosition(7, 7)},
            new PiecePosition{id="a6", pieceType=PieceType.BlackPawn, position=new BoardPosition(0, 6)},
            new PiecePosition{id="b6", pieceType=PieceType.BlackPawn, position=new BoardPosition(1, 6)},
            new PiecePosition{id="c6", pieceType=PieceType.BlackPawn, position=new BoardPosition(2, 6)},
            new PiecePosition{id="d6", pieceType=PieceType.BlackPawn, position=new BoardPosition(3, 6)},
            new PiecePosition{id="e6", pieceType=PieceType.BlackPawn, position=new BoardPosition(4, 6)},
            new PiecePosition{id="f6", pieceType=PieceType.BlackPawn, position=new BoardPosition(5, 6)},
            new PiecePosition{id="g6", pieceType=PieceType.BlackPawn, position=new BoardPosition(6, 6)},
            new PiecePosition{id="h6", pieceType=PieceType.BlackPawn, position=new BoardPosition(7, 6)},
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
