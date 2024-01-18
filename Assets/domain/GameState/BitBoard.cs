using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class BitBoard
{
  private static int[] lsb_64_table = new[]{
    63, 30,  3, 32, 59, 14, 11, 33,
    60, 24, 50,  9, 55, 19, 21, 34,
    61, 29,  2, 53, 51, 23, 41, 18,
    56, 28,  1, 43, 46, 27,  0, 35,
    62, 31, 58,  4,  5, 49, 54,  6,
    15, 52, 12, 40,  7, 42, 45, 16,
    25, 57, 48, 13, 10, 39,  8, 44,
    20, 47, 38, 22, 17, 37, 36, 26
  };

  private static int[] msb_64_table = new[]{
    0, 47,  1, 56, 48, 27,  2, 60,
    57, 49, 41, 37, 28, 16,  3, 61,
    54, 58, 35, 52, 50, 42, 21, 44,
    38, 32, 29, 23, 17, 11,  4, 62,
    46, 55, 26, 59, 40, 36, 15, 53,
    34, 51, 20, 43, 31, 22, 10, 45,
    25, 39, 14, 33, 19, 30,  9, 24,
    13, 18,  8, 12,  7,  6,  5, 63
  };

  private static ulong[] shiftPositiveAvoidWraps = new ulong[9]{
    0x0000000000000000,
    0x8080808080808080,
    0xc0c0c0c0c0c0c0c0,
    0xe0e0e0e0e0e0e0e0,
    0xf0f0f0f0f0f0f0f0,
    0xf8f8f8f8f8f8f8f8,
    0xfcfcfcfcfcfcfcfc,
    0xfefefefefefefefe,
    0xffffffffffffffff,
  };

  private static ulong[] shiftNegativeAvoidWraps = new ulong[9]{
    0x0000000000000000,
    0x0101010101010101,
    0x0303030303030303,
    0x0707070707070707,
    0x0f0f0f0f0f0f0f0f,
    0x1f1f1f1f1f1f1f1f,
    0x3f3f3f3f3f3f3f3f,
    0x7f7f7f7f7f7f7f7f,
    0xffffffffffffffff,
  };

  public static ulong firstColumn = 0x0101010101010101;
  public static ulong firstRow = 0x00000000000000FF;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int lsb(this ulong bitBoard)
  {
    unchecked
    {
      if (bitBoard == 0) return -1;
      bitBoard ^= bitBoard - 1;
      uint folded = ((uint)bitBoard) ^ (uint)(bitBoard >> 32);
      return lsb_64_table[folded * 0x78291ACF >> 26];
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int lsbUnchecked(this ulong bitBoard)
  {
    unchecked
    {
      bitBoard ^= bitBoard - 1;
      uint folded = ((uint)bitBoard) ^ (uint)(bitBoard >> 32);
      return lsb_64_table[folded * 0x78291ACF >> 26];
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int msb(this ulong bitBoard)
  {
    unchecked
    {
      if (bitBoard == 0) return -1;
      bitBoard |= bitBoard >> 1;
      bitBoard |= bitBoard >> 2;
      bitBoard |= bitBoard >> 4;
      bitBoard |= bitBoard >> 8;
      bitBoard |= bitBoard >> 16;
      bitBoard |= bitBoard >> 32;
      return msb_64_table[(bitBoard * 0x03f79d71b4cb0a89ul) >> 58];
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int msbUnchecked(this ulong bitBoard)
  {
    unchecked
    {
      bitBoard |= bitBoard >> 1;
      bitBoard |= bitBoard >> 2;
      bitBoard |= bitBoard >> 4;
      bitBoard |= bitBoard >> 8;
      bitBoard |= bitBoard >> 16;
      bitBoard |= bitBoard >> 32;
      return msb_64_table[(bitBoard * 0x03f79d71b4cb0a89ul) >> 58];
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ulong shiftColumn(this ulong bitBoard, int shift)
  {
    unchecked
    {
      return shift < 0 ? (bitBoard & ~shiftNegativeAvoidWraps[-shift]) >> (-shift) : (bitBoard & ~shiftPositiveAvoidWraps[shift]) << shift;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ulong shiftRow(this ulong bitBoard, int shift)
  {
    unchecked
    {
      return shift < 0 ? bitBoard >> (-8 * shift) : bitBoard << (8 * shift);
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int bitCount(this ulong bitBoard)
  {
    int count = 0;
    while (bitBoard != 0)
    {
      count++;
      bitBoard &= bitBoard - 1;
    }
    return count;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int bitCountLimit(this ulong bitBoard, int limit)
  {
    int count = 0;
    while (bitBoard != 0 && count < limit)
    {
      count++;
      bitBoard &= bitBoard - 1;
    }
    return count;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int[] extractIndices(this ulong bitBoard)
  {
    var bitCount = bitBoard.bitCount();
    var indices = new int[bitCount];
    for (var index = 0; index < bitCount; ++index)
    {
      var nextPiece = bitBoard.lsbUnchecked();
      bitBoard ^= 1ul << nextPiece;
      indices[index] = nextPiece;
    }

    return indices;
  }
}