using System.Collections.Generic;

public static class V8Precomputed
{
  public readonly static int[][] knightJumps = GenerateKnightJumps();

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
        validJumps.Add(V8BoardPosition.fromColRow(jump.col, jump.row));
      }

      allJumps[index] = validJumps.ToArray();
    }

    return allJumps;
  }
}