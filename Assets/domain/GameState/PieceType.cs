public enum PieceType
{
    Nothing = 0,
    WhitePawn,
    WhiteRook,
    WhiteKnight,
    WhiteBishop,
    WhiteQueen,
    WhiteKing,
    BlackPawn,
    BlackRook,
    BlackKnight,
    BlackBishop,
    BlackQueen,
    BlackKing,
}

public static class PieceTypeExtensions
{
    public static bool IsWhite(this PieceType pieceType)
    {
        return pieceType == PieceType.WhitePawn
        || pieceType == PieceType.WhiteRook
        || pieceType == PieceType.WhiteKnight
        || pieceType == PieceType.WhiteBishop
        || pieceType == PieceType.WhiteQueen
        || pieceType == PieceType.WhiteKing;
    }

    public static bool IsBlack(this PieceType pieceType)
    {
        return pieceType == PieceType.BlackPawn
        || pieceType == PieceType.BlackRook
        || pieceType == PieceType.BlackKnight
        || pieceType == PieceType.BlackBishop
        || pieceType == PieceType.BlackQueen
        || pieceType == PieceType.BlackKing;
    }

    public static bool IsKing(this PieceType pieceType)
    {
        return pieceType == PieceType.WhiteKing
        || pieceType == PieceType.BlackKing;
    }

    public static bool IsPawn(this PieceType pieceType)
    {
        return pieceType == PieceType.WhitePawn
        || pieceType == PieceType.BlackPawn;
    }

    public static bool IsQueen(this PieceType pieceType)
    {
        return pieceType == PieceType.WhiteQueen
        || pieceType == PieceType.BlackQueen;
    }

    public static bool IsBishop(this PieceType pieceType)
    {
        return pieceType == PieceType.WhiteBishop
        || pieceType == PieceType.BlackBishop;
    }

    public static bool IsRook(this PieceType pieceType)
    {
        return pieceType == PieceType.WhiteRook
        || pieceType == PieceType.BlackRook;
    }

    public static bool IsKnight(this PieceType pieceType)
    {
        return pieceType == PieceType.WhiteKnight
        || pieceType == PieceType.BlackKnight;
    }
}