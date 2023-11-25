using System.Diagnostics;

[DebuggerDisplay("{col}, {row}")]
public struct BoardPosition
{
    public int index { get; set; }

    public int col { get => index % 8; }
    public int row { get => index / 8; }

    public BoardPosition(int col, int row)
    {
        index = row * 8 + col;
    }

    public static bool IsInBoard(int col, int row)
    {
        return row >= 0 && row <= 7 && col >= 0 && col <= 7;
    }
}
