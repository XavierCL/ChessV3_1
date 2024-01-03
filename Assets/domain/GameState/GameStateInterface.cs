
using System.Collections.Generic;

public abstract class GameStateInterface
{
    public abstract int StaleTurns { get; }
    public abstract List<ReversibleMove> history { get; }
    public abstract BoardStateInterface BoardState { get; }
    public abstract Dictionary<BoardStateInterface, ushort> Snapshots { get; }

    public abstract IReadOnlyList<Move> getLegalMoves();
    public abstract ReversibleMove PlayMove(Move move);
    public abstract GameEndState GetGameEndState();
    public abstract void UndoMove();
    public string GetFen()
    {
        var fen = "";
        for (var row = 7; row >= 0; --row)
        {
            var emptySpaces = 0;
            for (var col = 0; col <= 7; ++col)
            {
                var pieceAtPosition = BoardState.piecePositions.Find(piece => piece.position.Equals(new BoardPosition(col, row)));
                if (pieceAtPosition == null)
                {
                    emptySpaces += 1;
                }
                else
                {
                    if (emptySpaces > 0)
                    {
                        fen += emptySpaces;
                        emptySpaces = 0;
                    }

                    switch (pieceAtPosition.pieceType)
                    {
                        case PieceType.WhitePawn:
                            fen += "P";
                            break;
                        case PieceType.WhiteRook:
                            fen += "R";
                            break;
                        case PieceType.WhiteKnight:
                            fen += "N";
                            break;
                        case PieceType.WhiteBishop:
                            fen += "B";
                            break;
                        case PieceType.WhiteQueen:
                            fen += "Q";
                            break;
                        case PieceType.WhiteKing:
                            fen += "K";
                            break;
                        case PieceType.BlackPawn:
                            fen += "p";
                            break;
                        case PieceType.BlackRook:
                            fen += "r";
                            break;
                        case PieceType.BlackKnight:
                            fen += "n";
                            break;
                        case PieceType.BlackBishop:
                            fen += "b";
                            break;
                        case PieceType.BlackQueen:
                            fen += "q";
                            break;
                        case PieceType.BlackKing:
                            fen += "k";
                            break;
                        default:
                            throw new System.Exception("Invalid piece type while building fen");
                    }
                }
            }

            if (emptySpaces > 0)
            {
                fen += emptySpaces;
            }

            if (row > 0) fen += "/";
        }

        var startingColor = BoardState.WhiteTurn ? "w" : "b";
        return $"{fen} {startingColor}";
    }
}
