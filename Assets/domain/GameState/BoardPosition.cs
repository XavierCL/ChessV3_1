using System.Diagnostics;

[DebuggerDisplay("{pretty}")]
public struct BoardPosition
{
    public int index { get; set; }

    public int col { get => index % 8; }
    public int row { get => index / 8; }
    public string pretty { get => ((char)('a' + col)).ToString() + ((char)('1' + row)).ToString(); }

    public BoardPosition(int col, int row)
    {
        index = row * 8 + col;
    }

    public static bool IsInBoard(int col, int row)
    {
        return row >= 0 && row <= 7 && col >= 0 && col <= 7;
    }

    public override int GetHashCode()
    {
        return index;
    }

    public override bool Equals(object obj)
    {
        var other = (BoardPosition)obj;
        return index == other.index;
    }
}
