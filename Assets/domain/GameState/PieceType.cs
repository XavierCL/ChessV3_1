public enum PieceType
{
    Nothing = 0,
    WhitePawn = 1,
    WhiteRook = 2,
    WhiteKnight = 3,
    WhiteBishop = 4,
    WhiteQueen = 5,
    WhiteKing = 6,
    BlackPawn = 9,
    BlackRook = 10,
    BlackKnight = 11,
    BlackBishop = 12,
    BlackQueen = 13,
    BlackKing = 14,
}

public static class PieceTypeExtensions
{
    public static bool IsWhite(this PieceType pieceType)
    {
        return pieceType != PieceType.Nothing && (((int)pieceType & 8) == 0);
    }

    public static bool IsBlack(this PieceType pieceType)
    {
        return ((int)pieceType & 8) == 8;
    }

    public static bool IsKing(this PieceType pieceType)
    {
        return ((int)pieceType & 7) == 6;
    }

    public static bool IsPawn(this PieceType pieceType)
    {
        return ((int)pieceType & 7) == 1;
    }

    public static bool IsQueen(this PieceType pieceType)
    {
        return ((int)pieceType & 7) == 5;
    }

    public static bool IsBishop(this PieceType pieceType)
    {
        return ((int)pieceType & 7) == 4;
    }

    public static bool IsRook(this PieceType pieceType)
    {
        return ((int)pieceType & 7) == 2;
    }

    public static bool IsKnight(this PieceType pieceType)
    {
        return ((int)pieceType & 7) == 3;
    }
}