using System.Runtime.CompilerServices;

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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int lsb(this ulong bitBoard)
  {
    unchecked
    {
      if (bitBoard == 0) return -1;
      uint folded;
      bitBoard ^= bitBoard - 1;
      folded = ((uint)bitBoard) ^ (uint)(bitBoard >> 32);
      return lsb_64_table[folded * 0x78291ACF >> 26];
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int msb(this ulong bitBoard)
  {
    unchecked
    {
      var copy = bitBoard;
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
}