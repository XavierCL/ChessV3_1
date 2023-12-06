using System;
using System.Runtime.CompilerServices;


[Flags]
public enum PieceType
{
    Nothing = 0,

    White = 64,
    Black = 128,

    Pawn = 1,
    Rook = 2,
    Knight = 4,
    Bishop = 8,
    Queen = 16,
    King = 32,

    WhitePawn = White | Pawn,
    WhiteRook = White | Rook,
    WhiteKnight = White | Knight,
    WhiteBishop = White | Bishop,
    WhiteQueen = White | Queen,
    WhiteKing = White | King,

    BlackPawn = Black | Pawn,
    BlackRook = Black | Rook,
    BlackKnight = Black | Knight,
    BlackBishop = Black | Bishop,
    BlackQueen = Black | Queen,
    BlackKing = Black | King,
}

public static class PieceTypeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhite(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.White);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBlack(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.Black);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKing(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.King);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPawn(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.Pawn);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsQueen(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.Queen); ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBishop(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.Bishop);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRook(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.Rook);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKnight(this PieceType pieceType)
    {
        return pieceType.HasFlag(PieceType.Knight);
    }
}