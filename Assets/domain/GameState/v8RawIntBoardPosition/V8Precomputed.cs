using System.Collections.Generic;

public static class V8Precomputed
{
  public readonly static int[][] knightJumps = new int[][] { new int[] { 17, 10 }, new int[] { 16, 18, 11 }, new int[] { 8, 17, 19, 12 }, new int[] { 9, 18, 20, 13 }, new int[] { 10, 19, 21, 14 }, new int[] { 11, 20, 22, 15 }, new int[] { 12, 21, 23 }, new int[] { 13, 22 }, new int[] { 25, 18, 2 }, new int[] { 24, 26, 19, 3 }, new int[] { 0, 16, 25, 27, 20, 4 }, new int[] { 1, 17, 26, 28, 21, 5 }, new int[] { 2, 18, 27, 29, 22, 6 }, new int[] { 3, 19, 28, 30, 23, 7 }, new int[] { 4, 20, 29, 31 }, new int[] { 5, 21, 30 }, new int[] { 33, 26, 10, 1 }, new int[] { 32, 34, 27, 11, 0, 2 }, new int[] { 8, 24, 33, 35, 28, 12, 1, 3 }, new int[] { 9, 25, 34, 36, 29, 13, 2, 4 }, new int[] { 10, 26, 35, 37, 30, 14, 3, 5 }, new int[] { 11, 27, 36, 38, 31, 15, 4, 6 }, new int[] { 12, 28, 37, 39, 5, 7 }, new int[] { 13, 29, 38, 6 }, new int[] { 41, 34, 18, 9 }, new int[] { 40, 42, 35, 19, 8, 10 }, new int[] { 16, 32, 41, 43, 36, 20, 9, 11 }, new int[] { 17, 33, 42, 44, 37, 21, 10, 12 }, new int[] { 18, 34, 43, 45, 38, 22, 11, 13 }, new int[] { 19, 35, 44, 46, 39, 23, 12, 14 }, new int[] { 20, 36, 45, 47, 13, 15 }, new int[] { 21, 37, 46, 14 }, new int[] { 49, 42, 26, 17 }, new int[] { 48, 50, 43, 27, 16, 18 }, new int[] { 24, 40, 49, 51, 44, 28, 17, 19 }, new int[] { 25, 41, 50, 52, 45, 29, 18, 20 }, new int[] { 26, 42, 51, 53, 46, 30, 19, 21 }, new int[] { 27, 43, 52, 54, 47, 31, 20, 22 }, new int[] { 28, 44, 53, 55, 21, 23 }, new int[] { 29, 45, 54, 22 }, new int[] { 57, 50, 34, 25 }, new int[] { 56, 58, 51, 35, 24, 26 }, new int[] { 32, 48, 57, 59, 52, 36, 25, 27 }, new int[] { 33, 49, 58, 60, 53, 37, 26, 28 }, new int[] { 34, 50, 59, 61, 54, 38, 27, 29 }, new int[] { 35, 51, 60, 62, 55, 39, 28, 30 }, new int[] { 36, 52, 61, 63, 29, 31 }, new int[] { 37, 53, 62, 30 }, new int[] { 58, 42, 33 }, new int[] { 59, 43, 32, 34 }, new int[] { 40, 56, 60, 44, 33, 35 }, new int[] { 41, 57, 61, 45, 34, 36 }, new int[] { 42, 58, 62, 46, 35, 37 }, new int[] { 43, 59, 63, 47, 36, 38 }, new int[] { 44, 60, 37, 39 }, new int[] { 45, 61, 38 }, new int[] { 50, 41 }, new int[] { 51, 40, 42 }, new int[] { 48, 52, 41, 43 }, new int[] { 49, 53, 42, 44 }, new int[] { 50, 54, 43, 45 }, new int[] { 51, 55, 44, 46 }, new int[] { 52, 45, 47 }, new int[] { 53, 46 } }; // GenerateKnightJumps();

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
}