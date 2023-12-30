using System;
using System.Runtime.CompilerServices;


[Flags]
public enum CastleFlags
{
  Nothing = 0,
  WhiteKing = 1,
  WhiteQueen = 2,
  BlackKing = 4,
  BlackQueen = 8,
  All = WhiteKing | WhiteQueen | BlackKing | BlackQueen,
  White = WhiteKing | WhiteQueen,
  Black = BlackKing | BlackQueen
}

public static class CastleFlagExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsWhite(this CastleFlags castleFlag)
  {
    return (castleFlag & CastleFlags.White) == castleFlag;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsBlack(this CastleFlags castleFlag)
  {
    return (castleFlag & CastleFlags.Black) == castleFlag;
  }
}