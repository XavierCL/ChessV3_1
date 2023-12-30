using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class V11Precomputed
{
  // knightJumps[sourcePosition] = int[] destinations
  public readonly static int[][] knightJumps = new int[][] { new int[] { 17, 10 }, new int[] { 16, 18, 11 }, new int[] { 8, 17, 19, 12 }, new int[] { 9, 18, 20, 13 }, new int[] { 10, 19, 21, 14 }, new int[] { 11, 20, 22, 15 }, new int[] { 12, 21, 23 }, new int[] { 13, 22 }, new int[] { 25, 18, 2 }, new int[] { 24, 26, 19, 3 }, new int[] { 0, 16, 25, 27, 20, 4 }, new int[] { 1, 17, 26, 28, 21, 5 }, new int[] { 2, 18, 27, 29, 22, 6 }, new int[] { 3, 19, 28, 30, 23, 7 }, new int[] { 4, 20, 29, 31 }, new int[] { 5, 21, 30 }, new int[] { 33, 26, 10, 1 }, new int[] { 32, 34, 27, 11, 0, 2 }, new int[] { 8, 24, 33, 35, 28, 12, 1, 3 }, new int[] { 9, 25, 34, 36, 29, 13, 2, 4 }, new int[] { 10, 26, 35, 37, 30, 14, 3, 5 }, new int[] { 11, 27, 36, 38, 31, 15, 4, 6 }, new int[] { 12, 28, 37, 39, 5, 7 }, new int[] { 13, 29, 38, 6 }, new int[] { 41, 34, 18, 9 }, new int[] { 40, 42, 35, 19, 8, 10 }, new int[] { 16, 32, 41, 43, 36, 20, 9, 11 }, new int[] { 17, 33, 42, 44, 37, 21, 10, 12 }, new int[] { 18, 34, 43, 45, 38, 22, 11, 13 }, new int[] { 19, 35, 44, 46, 39, 23, 12, 14 }, new int[] { 20, 36, 45, 47, 13, 15 }, new int[] { 21, 37, 46, 14 }, new int[] { 49, 42, 26, 17 }, new int[] { 48, 50, 43, 27, 16, 18 }, new int[] { 24, 40, 49, 51, 44, 28, 17, 19 }, new int[] { 25, 41, 50, 52, 45, 29, 18, 20 }, new int[] { 26, 42, 51, 53, 46, 30, 19, 21 }, new int[] { 27, 43, 52, 54, 47, 31, 20, 22 }, new int[] { 28, 44, 53, 55, 21, 23 }, new int[] { 29, 45, 54, 22 }, new int[] { 57, 50, 34, 25 }, new int[] { 56, 58, 51, 35, 24, 26 }, new int[] { 32, 48, 57, 59, 52, 36, 25, 27 }, new int[] { 33, 49, 58, 60, 53, 37, 26, 28 }, new int[] { 34, 50, 59, 61, 54, 38, 27, 29 }, new int[] { 35, 51, 60, 62, 55, 39, 28, 30 }, new int[] { 36, 52, 61, 63, 29, 31 }, new int[] { 37, 53, 62, 30 }, new int[] { 58, 42, 33 }, new int[] { 59, 43, 32, 34 }, new int[] { 40, 56, 60, 44, 33, 35 }, new int[] { 41, 57, 61, 45, 34, 36 }, new int[] { 42, 58, 62, 46, 35, 37 }, new int[] { 43, 59, 63, 47, 36, 38 }, new int[] { 44, 60, 37, 39 }, new int[] { 45, 61, 38 }, new int[] { 50, 41 }, new int[] { 51, 40, 42 }, new int[] { 48, 52, 41, 43 }, new int[] { 49, 53, 42, 44 }, new int[] { 50, 54, 43, 45 }, new int[] { 51, 55, 44, 46 }, new int[] { 52, 45, 47 }, new int[] { 53, 46 } }; // GenerateKnightJumps();

  // bitBoardAtTopRightRay[sourcePosition] = ulong bitboard
  public readonly static ulong[] bitBoardAtTopLeftRay = new ulong[] { 0x0ul, 0x100ul, 0x10200ul, 0x1020400ul, 0x102040800ul, 0x10204081000ul, 0x1020408102000ul, 0x102040810204000ul, 0x0ul, 0x10000ul, 0x1020000ul, 0x102040000ul, 0x10204080000ul, 0x1020408100000ul, 0x102040810200000ul, 0x204081020400000ul, 0x0ul, 0x1000000ul, 0x102000000ul, 0x10204000000ul, 0x1020408000000ul, 0x102040810000000ul, 0x204081020000000ul, 0x408102040000000ul, 0x0ul, 0x100000000ul, 0x10200000000ul, 0x1020400000000ul, 0x102040800000000ul, 0x204081000000000ul, 0x408102000000000ul, 0x810204000000000ul, 0x0ul, 0x10000000000ul, 0x1020000000000ul, 0x102040000000000ul, 0x204080000000000ul, 0x408100000000000ul, 0x810200000000000ul, 0x1020400000000000ul, 0x0ul, 0x1000000000000ul, 0x102000000000000ul, 0x204000000000000ul, 0x408000000000000ul, 0x810000000000000ul, 0x1020000000000000ul, 0x2040000000000000ul, 0x0ul, 0x100000000000000ul, 0x200000000000000ul, 0x400000000000000ul, 0x800000000000000ul, 0x1000000000000000ul, 0x2000000000000000ul, 0x4000000000000000ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul }; // GenerateBitBoardAtRay(-1, 1);
  public readonly static ulong[] bitBoardAtTopRay = new ulong[] { 0x101010101010100ul, 0x202020202020200ul, 0x404040404040400ul, 0x808080808080800ul, 0x1010101010101000ul, 0x2020202020202000ul, 0x4040404040404000ul, 0x8080808080808000ul, 0x101010101010000ul, 0x202020202020000ul, 0x404040404040000ul, 0x808080808080000ul, 0x1010101010100000ul, 0x2020202020200000ul, 0x4040404040400000ul, 0x8080808080800000ul, 0x101010101000000ul, 0x202020202000000ul, 0x404040404000000ul, 0x808080808000000ul, 0x1010101010000000ul, 0x2020202020000000ul, 0x4040404040000000ul, 0x8080808080000000ul, 0x101010100000000ul, 0x202020200000000ul, 0x404040400000000ul, 0x808080800000000ul, 0x1010101000000000ul, 0x2020202000000000ul, 0x4040404000000000ul, 0x8080808000000000ul, 0x101010000000000ul, 0x202020000000000ul, 0x404040000000000ul, 0x808080000000000ul, 0x1010100000000000ul, 0x2020200000000000ul, 0x4040400000000000ul, 0x8080800000000000ul, 0x101000000000000ul, 0x202000000000000ul, 0x404000000000000ul, 0x808000000000000ul, 0x1010000000000000ul, 0x2020000000000000ul, 0x4040000000000000ul, 0x8080000000000000ul, 0x100000000000000ul, 0x200000000000000ul, 0x400000000000000ul, 0x800000000000000ul, 0x1000000000000000ul, 0x2000000000000000ul, 0x4000000000000000ul, 0x8000000000000000ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul }; // GenerateBitBoardAtRay(0, 1);
  public readonly static ulong[] bitBoardAtTopRightRay = new ulong[] { 0x8040201008040200ul, 0x80402010080400ul, 0x804020100800ul, 0x8040201000ul, 0x80402000ul, 0x804000ul, 0x8000ul, 0x0ul, 0x4020100804020000ul, 0x8040201008040000ul, 0x80402010080000ul, 0x804020100000ul, 0x8040200000ul, 0x80400000ul, 0x800000ul, 0x0ul, 0x2010080402000000ul, 0x4020100804000000ul, 0x8040201008000000ul, 0x80402010000000ul, 0x804020000000ul, 0x8040000000ul, 0x80000000ul, 0x0ul, 0x1008040200000000ul, 0x2010080400000000ul, 0x4020100800000000ul, 0x8040201000000000ul, 0x80402000000000ul, 0x804000000000ul, 0x8000000000ul, 0x0ul, 0x804020000000000ul, 0x1008040000000000ul, 0x2010080000000000ul, 0x4020100000000000ul, 0x8040200000000000ul, 0x80400000000000ul, 0x800000000000ul, 0x0ul, 0x402000000000000ul, 0x804000000000000ul, 0x1008000000000000ul, 0x2010000000000000ul, 0x4020000000000000ul, 0x8040000000000000ul, 0x80000000000000ul, 0x0ul, 0x200000000000000ul, 0x400000000000000ul, 0x800000000000000ul, 0x1000000000000000ul, 0x2000000000000000ul, 0x4000000000000000ul, 0x8000000000000000ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul }; // GenerateBitBoardAtRay(1, 1);
  public readonly static ulong[] bitBoardAtRightRay = new ulong[] { 0xFEul, 0xFCul, 0xF8ul, 0xF0ul, 0xE0ul, 0xC0ul, 0x80ul, 0x0ul, 0xFE00ul, 0xFC00ul, 0xF800ul, 0xF000ul, 0xE000ul, 0xC000ul, 0x8000ul, 0x0ul, 0xFE0000ul, 0xFC0000ul, 0xF80000ul, 0xF00000ul, 0xE00000ul, 0xC00000ul, 0x800000ul, 0x0ul, 0xFE000000ul, 0xFC000000ul, 0xF8000000ul, 0xF0000000ul, 0xE0000000ul, 0xC0000000ul, 0x80000000ul, 0x0ul, 0xFE00000000ul, 0xFC00000000ul, 0xF800000000ul, 0xF000000000ul, 0xE000000000ul, 0xC000000000ul, 0x8000000000ul, 0x0ul, 0xFE0000000000ul, 0xFC0000000000ul, 0xF80000000000ul, 0xF00000000000ul, 0xE00000000000ul, 0xC00000000000ul, 0x800000000000ul, 0x0ul, 0xFE000000000000ul, 0xFC000000000000ul, 0xF8000000000000ul, 0xF0000000000000ul, 0xE0000000000000ul, 0xC0000000000000ul, 0x80000000000000ul, 0x0ul, 0xFE00000000000000ul, 0xFC00000000000000ul, 0xF800000000000000ul, 0xF000000000000000ul, 0xE000000000000000ul, 0xC000000000000000ul, 0x8000000000000000ul, 0x0ul }; // GenerateBitBoardAtRay(1, 0);
  public readonly static ulong[] bitBoardAtBottomRightRay = new ulong[] { 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x2ul, 0x4ul, 0x8ul, 0x10ul, 0x20ul, 0x40ul, 0x80ul, 0x0ul, 0x204ul, 0x408ul, 0x810ul, 0x1020ul, 0x2040ul, 0x4080ul, 0x8000ul, 0x0ul, 0x20408ul, 0x40810ul, 0x81020ul, 0x102040ul, 0x204080ul, 0x408000ul, 0x800000ul, 0x0ul, 0x2040810ul, 0x4081020ul, 0x8102040ul, 0x10204080ul, 0x20408000ul, 0x40800000ul, 0x80000000ul, 0x0ul, 0x204081020ul, 0x408102040ul, 0x810204080ul, 0x1020408000ul, 0x2040800000ul, 0x4080000000ul, 0x8000000000ul, 0x0ul, 0x20408102040ul, 0x40810204080ul, 0x81020408000ul, 0x102040800000ul, 0x204080000000ul, 0x408000000000ul, 0x800000000000ul, 0x0ul, 0x2040810204080ul, 0x4081020408000ul, 0x8102040800000ul, 0x10204080000000ul, 0x20408000000000ul, 0x40800000000000ul, 0x80000000000000ul, 0x0ul }; // GenerateBitBoardAtRay(1, -1);
  public readonly static ulong[] bitBoardAtBottomRay = new ulong[] { 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x1ul, 0x2ul, 0x4ul, 0x8ul, 0x10ul, 0x20ul, 0x40ul, 0x80ul, 0x101ul, 0x202ul, 0x404ul, 0x808ul, 0x1010ul, 0x2020ul, 0x4040ul, 0x8080ul, 0x10101ul, 0x20202ul, 0x40404ul, 0x80808ul, 0x101010ul, 0x202020ul, 0x404040ul, 0x808080ul, 0x1010101ul, 0x2020202ul, 0x4040404ul, 0x8080808ul, 0x10101010ul, 0x20202020ul, 0x40404040ul, 0x80808080ul, 0x101010101ul, 0x202020202ul, 0x404040404ul, 0x808080808ul, 0x1010101010ul, 0x2020202020ul, 0x4040404040ul, 0x8080808080ul, 0x10101010101ul, 0x20202020202ul, 0x40404040404ul, 0x80808080808ul, 0x101010101010ul, 0x202020202020ul, 0x404040404040ul, 0x808080808080ul, 0x1010101010101ul, 0x2020202020202ul, 0x4040404040404ul, 0x8080808080808ul, 0x10101010101010ul, 0x20202020202020ul, 0x40404040404040ul, 0x80808080808080ul }; // GenerateBitBoardAtRay(0, -1);
  public readonly static ulong[] bitBoardAtBottomLeftRay = new ulong[] { 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x1ul, 0x2ul, 0x4ul, 0x8ul, 0x10ul, 0x20ul, 0x40ul, 0x0ul, 0x100ul, 0x201ul, 0x402ul, 0x804ul, 0x1008ul, 0x2010ul, 0x4020ul, 0x0ul, 0x10000ul, 0x20100ul, 0x40201ul, 0x80402ul, 0x100804ul, 0x201008ul, 0x402010ul, 0x0ul, 0x1000000ul, 0x2010000ul, 0x4020100ul, 0x8040201ul, 0x10080402ul, 0x20100804ul, 0x40201008ul, 0x0ul, 0x100000000ul, 0x201000000ul, 0x402010000ul, 0x804020100ul, 0x1008040201ul, 0x2010080402ul, 0x4020100804ul, 0x0ul, 0x10000000000ul, 0x20100000000ul, 0x40201000000ul, 0x80402010000ul, 0x100804020100ul, 0x201008040201ul, 0x402010080402ul, 0x0ul, 0x1000000000000ul, 0x2010000000000ul, 0x4020100000000ul, 0x8040201000000ul, 0x10080402010000ul, 0x20100804020100ul, 0x40201008040201ul };// GenerateBitBoardAtRay(-1, -1);
  public readonly static ulong[] bitBoardAtLeftRay = new ulong[] { 0x0ul, 0x1ul, 0x3ul, 0x7ul, 0xFul, 0x1Ful, 0x3Ful, 0x7Ful, 0x0ul, 0x100ul, 0x300ul, 0x700ul, 0xF00ul, 0x1F00ul, 0x3F00ul, 0x7F00ul, 0x0ul, 0x10000ul, 0x30000ul, 0x70000ul, 0xF0000ul, 0x1F0000ul, 0x3F0000ul, 0x7F0000ul, 0x0ul, 0x1000000ul, 0x3000000ul, 0x7000000ul, 0xF000000ul, 0x1F000000ul, 0x3F000000ul, 0x7F000000ul, 0x0ul, 0x100000000ul, 0x300000000ul, 0x700000000ul, 0xF00000000ul, 0x1F00000000ul, 0x3F00000000ul, 0x7F00000000ul, 0x0ul, 0x10000000000ul, 0x30000000000ul, 0x70000000000ul, 0xF0000000000ul, 0x1F0000000000ul, 0x3F0000000000ul, 0x7F0000000000ul, 0x0ul, 0x1000000000000ul, 0x3000000000000ul, 0x7000000000000ul, 0xF000000000000ul, 0x1F000000000000ul, 0x3F000000000000ul, 0x7F000000000000ul, 0x0ul, 0x100000000000000ul, 0x300000000000000ul, 0x700000000000000ul, 0xF00000000000000ul, 0x1F00000000000000ul, 0x3F00000000000000ul, 0x7F00000000000000ul }; // GenerateBitBoardAtRay(-1, 0);

  public readonly static ulong[][] bitBoardAtRayBranches = new ulong[][] {
    bitBoardAtBottomLeftRay, bitBoardAtLeftRay, bitBoardAtTopLeftRay, bitBoardAtBottomRay, new ulong[] {}, bitBoardAtTopRay, bitBoardAtBottomRightRay, bitBoardAtRightRay, bitBoardAtTopRightRay
  };

  private static int[][] GenerateKnightJumps()
  {
    var allJumps = new int[64][];
    for (var index = 0; index < 64; ++index)
    {
      var position = new BoardPosition(index);
      var jumps = new[]{
        new { col = position.col - 2, row = position.row - 1},
        new { col = position.col - 2, row = position.row + 1},
        new { col = position.col - 1, row = position.row + 2},
        new { col = position.col + 1, row = position.row + 2},
        new { col = position.col + 2, row = position.row + 1},
        new { col = position.col + 2, row = position.row - 1},
        new { col = position.col - 1, row = position.row - 2},
        new { col = position.col + 1, row = position.row - 2},
      };

      var validJumps = new List<int>(8);

      for (var jumpIndex = 0; jumpIndex < jumps.Length; ++jumpIndex)
      {
        var jump = jumps[jumpIndex];
        if (!BoardPosition.IsInBoard(jump.col, jump.row)) continue;
        validJumps.Add(BoardPosition.fromColRow(jump.col, jump.row));
      }

      allJumps[index] = validJumps.ToArray();
    }

    Debug.Log(string.Join(", ", allJumps.Select(validJumps => "new int[] {" + string.Join(", ", validJumps) + "}")));

    return allJumps;
  }

  private static ulong[] GenerateBitBoardAtRay(int colIncrement, int rowIncrement)
  {
    var bitBoards = new ulong[64];

    for (var sourcePosition = 0; sourcePosition < 64; ++sourcePosition)
    {
      ulong bitBoard = 0;
      var col = sourcePosition.getCol() + colIncrement;
      var row = sourcePosition.getRow() + rowIncrement;

      while (col <= 7 && col >= 0 && row <= 7 && row >= 0)
      {
        bitBoard |= 1UL << BoardPosition.fromColRow(col, row);

        col += colIncrement;
        row += rowIncrement;
      }

      bitBoards[sourcePosition] = bitBoard;
    }

    Debug.Log($"{colIncrement}, {rowIncrement}\n" + string.Join(", ", bitBoards.Select(b => "0x" + b.ToString("X") + "ul")));

    return bitBoards;
  }
}