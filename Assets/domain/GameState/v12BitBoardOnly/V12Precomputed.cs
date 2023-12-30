using System.Linq;
using UnityEngine;

public static class V12Precomputed
{
  // knightBitBoards[sourcePosition] = ulong destinations
  public readonly static ulong[] knightBitBoards = new ulong[] { 0x20400ul, 0x50800ul, 0xA1100ul, 0x142200ul, 0x284400ul, 0x508800ul, 0xA01000ul, 0x402000ul, 0x2040004ul, 0x5080008ul, 0xA110011ul, 0x14220022ul, 0x28440044ul, 0x50880088ul, 0xA0100010ul, 0x40200020ul, 0x204000402ul, 0x508000805ul, 0xA1100110Aul, 0x1422002214ul, 0x2844004428ul, 0x5088008850ul, 0xA0100010A0ul, 0x4020002040ul, 0x20400040200ul, 0x50800080500ul, 0xA1100110A00ul, 0x142200221400ul, 0x284400442800ul, 0x508800885000ul, 0xA0100010A000ul, 0x402000204000ul, 0x2040004020000ul, 0x5080008050000ul, 0xA1100110A0000ul, 0x14220022140000ul, 0x28440044280000ul, 0x50880088500000ul, 0xA0100010A00000ul, 0x40200020400000ul, 0x204000402000000ul, 0x508000805000000ul, 0xA1100110A000000ul, 0x1422002214000000ul, 0x2844004428000000ul, 0x5088008850000000ul, 0xA0100010A0000000ul, 0x4020002040000000ul, 0x400040200000000ul, 0x800080500000000ul, 0x1100110A00000000ul, 0x2200221400000000ul, 0x4400442800000000ul, 0x8800885000000000ul, 0x100010A000000000ul, 0x2000204000000000ul, 0x4020000000000ul, 0x8050000000000ul, 0x110A0000000000ul, 0x22140000000000ul, 0x44280000000000ul, 0x88500000000000ul, 0x10A00000000000ul, 0x20400000000000ul }; // GenerateKnightBitBoards();

  // kingBitBoards[sourcePosition] = ulong destinations
  public readonly static ulong[] kingBitBoards = new ulong[] { 0x302ul, 0x705ul, 0xE0Aul, 0x1C14ul, 0x3828ul, 0x7050ul, 0xE0A0ul, 0xC040ul, 0x30203ul, 0x70507ul, 0xE0A0Eul, 0x1C141Cul, 0x382838ul, 0x705070ul, 0xE0A0E0ul, 0xC040C0ul, 0x3020300ul, 0x7050700ul, 0xE0A0E00ul, 0x1C141C00ul, 0x38283800ul, 0x70507000ul, 0xE0A0E000ul, 0xC040C000ul, 0x302030000ul, 0x705070000ul, 0xE0A0E0000ul, 0x1C141C0000ul, 0x3828380000ul, 0x7050700000ul, 0xE0A0E00000ul, 0xC040C00000ul, 0x30203000000ul, 0x70507000000ul, 0xE0A0E000000ul, 0x1C141C000000ul, 0x382838000000ul, 0x705070000000ul, 0xE0A0E0000000ul, 0xC040C0000000ul, 0x3020300000000ul, 0x7050700000000ul, 0xE0A0E00000000ul, 0x1C141C00000000ul, 0x38283800000000ul, 0x70507000000000ul, 0xE0A0E000000000ul, 0xC040C000000000ul, 0x302030000000000ul, 0x705070000000000ul, 0xE0A0E0000000000ul, 0x1C141C0000000000ul, 0x3828380000000000ul, 0x7050700000000000ul, 0xE0A0E00000000000ul, 0xC040C00000000000ul, 0x203000000000000ul, 0x507000000000000ul, 0xA0E000000000000ul, 0x141C000000000000ul, 0x2838000000000000ul, 0x5070000000000000ul, 0xA0E0000000000000ul, 0x40C0000000000000ul }; // GenerateKingBitBoards();

  // kingBitBoards[sourcePosition] = ulong destinations
  public readonly static ulong[] whitePawnCaptureBitBoards = new ulong[] { 0x200ul, 0x500ul, 0xA00ul, 0x1400ul, 0x2800ul, 0x5000ul, 0xA000ul, 0x4000ul, 0x20000ul, 0x50000ul, 0xA0000ul, 0x140000ul, 0x280000ul, 0x500000ul, 0xA00000ul, 0x400000ul, 0x2000000ul, 0x5000000ul, 0xA000000ul, 0x14000000ul, 0x28000000ul, 0x50000000ul, 0xA0000000ul, 0x40000000ul, 0x200000000ul, 0x500000000ul, 0xA00000000ul, 0x1400000000ul, 0x2800000000ul, 0x5000000000ul, 0xA000000000ul, 0x4000000000ul, 0x20000000000ul, 0x50000000000ul, 0xA0000000000ul, 0x140000000000ul, 0x280000000000ul, 0x500000000000ul, 0xA00000000000ul, 0x400000000000ul, 0x2000000000000ul, 0x5000000000000ul, 0xA000000000000ul, 0x14000000000000ul, 0x28000000000000ul, 0x50000000000000ul, 0xA0000000000000ul, 0x40000000000000ul, 0x200000000000000ul, 0x500000000000000ul, 0xA00000000000000ul, 0x1400000000000000ul, 0x2800000000000000ul, 0x5000000000000000ul, 0xA000000000000000ul, 0x4000000000000000ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul }; // GeneratePawnCaptureBitBoards(true);

  // kingBitBoards[sourcePosition] = ulong destinations
  public readonly static ulong[] blackPawnCaptureBitBoards = new ulong[] { 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x0ul, 0x2ul, 0x5ul, 0xAul, 0x14ul, 0x28ul, 0x50ul, 0xA0ul, 0x40ul, 0x200ul, 0x500ul, 0xA00ul, 0x1400ul, 0x2800ul, 0x5000ul, 0xA000ul, 0x4000ul, 0x20000ul, 0x50000ul, 0xA0000ul, 0x140000ul, 0x280000ul, 0x500000ul, 0xA00000ul, 0x400000ul, 0x2000000ul, 0x5000000ul, 0xA000000ul, 0x14000000ul, 0x28000000ul, 0x50000000ul, 0xA0000000ul, 0x40000000ul, 0x200000000ul, 0x500000000ul, 0xA00000000ul, 0x1400000000ul, 0x2800000000ul, 0x5000000000ul, 0xA000000000ul, 0x4000000000ul, 0x20000000000ul, 0x50000000000ul, 0xA0000000000ul, 0x140000000000ul, 0x280000000000ul, 0x500000000000ul, 0xA00000000000ul, 0x400000000000ul, 0x2000000000000ul, 0x5000000000000ul, 0xA000000000000ul, 0x14000000000000ul, 0x28000000000000ul, 0x50000000000000ul, 0xA0000000000000ul, 0x40000000000000ul }; // GeneratePawnCaptureBitBoards(false);

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

  private static ulong[] GenerateKnightBitBoards()
  {
    var allBitBoards = new ulong[64];
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

      for (var jumpIndex = 0; jumpIndex < jumps.Length; ++jumpIndex)
      {
        var jump = jumps[jumpIndex];
        if (!BoardPosition.IsInBoard(jump.col, jump.row)) continue;
        allBitBoards[index] |= BoardPosition.fromColRow(jump.col, jump.row).toBitBoard();
      }
    }

    Debug.Log(string.Join(", ", allBitBoards.Select(bitBoard => "0x" + bitBoard.ToString("X") + "ul")));

    return allBitBoards;
  }

  private static ulong[] GenerateKingBitBoards()
  {
    var allBitBoards = new ulong[64];
    for (var index = 0; index < 64; ++index)
    {
      var position = new BoardPosition(index);
      var jumps = new[]{
        new { col = position.col - 1, row = position.row - 1},
        new { col = position.col - 1, row = position.row },
        new { col = position.col - 1, row = position.row + 1},
        new { col = position.col, row = position.row + 1},
        new { col = position.col + 1, row = position.row + 1},
        new { col = position.col + 1, row = position.row },
        new { col = position.col + 1, row = position.row - 1},
        new { col = position.col, row = position.row - 1},
      };

      for (var jumpIndex = 0; jumpIndex < jumps.Length; ++jumpIndex)
      {
        var jump = jumps[jumpIndex];
        if (!BoardPosition.IsInBoard(jump.col, jump.row)) continue;
        allBitBoards[index] |= BoardPosition.fromColRow(jump.col, jump.row).toBitBoard();
      }
    }

    Debug.Log(string.Join(", ", allBitBoards.Select(bitBoard => "0x" + bitBoard.ToString("X") + "ul")));

    return allBitBoards;
  }

  private static ulong[] GeneratePawnCaptureBitBoards(bool isWhite)
  {
    var shift = isWhite ? 1 : -1;
    var allBitBoards = new ulong[64];
    for (var index = 0; index < 64; ++index)
    {
      var position = new BoardPosition(index);
      var jumps = new[]{
        new { col = position.col + 1, row = position.row + shift },
        new { col = position.col - 1, row = position.row + shift },
      };

      for (var jumpIndex = 0; jumpIndex < jumps.Length; ++jumpIndex)
      {
        var jump = jumps[jumpIndex];
        if (!BoardPosition.IsInBoard(jump.col, jump.row)) continue;
        allBitBoards[index] |= BoardPosition.fromColRow(jump.col, jump.row).toBitBoard();
      }
    }

    Debug.Log(string.Join(", ", allBitBoards.Select(bitBoard => "0x" + bitBoard.ToString("X") + "ul")));

    return allBitBoards;
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