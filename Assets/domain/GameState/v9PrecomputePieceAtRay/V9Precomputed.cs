using System.Collections.Generic;

public static class V9Precomputed
{
  // knightJumps[sourcePosition] = int[] destinations
  public readonly static int[][] knightJumps = GenerateKnightJumps();

  // bitBoardAtTopRightRay[sourcePosition] = ulong bitboard
  public readonly static ulong[] bitBoardAtTopRightRay = GenerateBitBoardAtRay(-1, 1);

  // firstPieceAtTopRightBitBoard[sourcePosition][bitboard] = int position (-1 if none)
  public readonly static Dictionary<ulong, int>[] firstPieceAtTopRightBitBoard = GenerateFirstPieceAtBitBoard(GenerateBitBoardAtRay(-1, 1), true);

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

    return bitBoards;
  }

  private static Dictionary<ulong, int>[] GenerateFirstPieceAtBitBoard(ulong[] fullBitBoards, bool lowFirst)
  {
    var allFirstPieces = new Dictionary<ulong, int>[64];

    for (var sourcePosition = 0; sourcePosition < 64; ++sourcePosition)
    {
      var firstPieces = new Dictionary<ulong, int>();

      bitBoards[sourcePosition] = bitBoard;
    }

    return allFirstPieces;
  }
}