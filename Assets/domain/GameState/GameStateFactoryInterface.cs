using System.Collections.Generic;

public abstract class GameStateFactoryInterface
{
    public abstract GameStateInterface StartingPosition();
    public abstract GameStateInterface FromGameState(GameStateInterface gameState);
    public abstract GameStateInterface FromFen(string fen);

    protected List<PiecePosition> FenToPiecePositions(string fen)
    {
        var piecePositions = new List<PiecePosition>();

        var piecesPerRank = fen.Split(" ")[0].Split("/");
        var row = 7;
        var pieceId = 0;
        foreach (var rank in piecesPerRank)
        {
            var readingColumn = 0;
            var col = 0;
            while (col < 8)
            {
                var character = rank[readingColumn].ToString();
                if (int.TryParse(character, out var spaces))
                {
                    col += spaces;
                }
                else
                {
                    switch (character)
                    {
                        case "P":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.WhitePawn, new BoardPosition(col, row)));
                            break;
                        case "N":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.WhiteKnight, new BoardPosition(col, row)));
                            break;
                        case "B":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.WhiteBishop, new BoardPosition(col, row)));
                            break;
                        case "R":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.WhiteRook, new BoardPosition(col, row)));
                            break;
                        case "Q":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.WhiteQueen, new BoardPosition(col, row)));
                            break;
                        case "K":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.WhiteKing, new BoardPosition(col, row)));
                            break;
                        case "p":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.BlackPawn, new BoardPosition(col, row)));
                            break;
                        case "n":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.BlackKnight, new BoardPosition(col, row)));
                            break;
                        case "b":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.BlackBishop, new BoardPosition(col, row)));
                            break;
                        case "r":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.BlackRook, new BoardPosition(col, row)));
                            break;
                        case "q":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.BlackQueen, new BoardPosition(col, row)));
                            break;
                        case "k":
                            piecePositions.Add(new PiecePosition(pieceId.ToString(), PieceType.BlackKing, new BoardPosition(col, row)));
                            break;
                        default:
                            throw new System.Exception("Could not parse fen character");
                    }

                    ++col;
                }

                ++readingColumn;
            }

            --row;
        }

        return piecePositions;
    }

    protected bool FenToWhite(string fen)
    {
        var fens = fen.Split(" ");
        if (fens.Length < 2) return true;
        var firstTurn = fens[1];
        switch (firstTurn)
        {
            case "w":
                return true;
            case "b":
                return false;
            default:
                throw new System.Exception("Invalid fen starting color");
        }
    }

    protected Castling FenToCastle(string fen)
    {
        var fens = fen.Split(" ");
        if (fens.Length < 3) return 0;
        var fenCastling = fens[2];
        if (fenCastling == "-") return 0;
        Castling castling = 0;
        if (fenCastling.Contains("K")) castling |= Castling.WhiteKing;
        if (fenCastling.Contains("Q")) castling |= Castling.WhiteQueen;
        if (fenCastling.Contains("k")) castling |= Castling.BlackKing;
        if (fenCastling.Contains("q")) castling |= Castling.BlackQueen;
        return castling;
    }
}
