using System.Diagnostics;

[DebuggerDisplay("{pretty}")]
public struct BoardPosition
{
    public readonly int index;

    public int col { get => index % 8; }
    public int row { get => index / 8; }
    public string pretty { get => $"{(char)('a' + col)}{(char)('1' + row)}"; }

    public BoardPosition(int col, int row)
    {
        index = row * 8 + col;
    }

    public BoardPosition(int index)
    {
        this.index = index;
    }

    public static bool IsInBoard(int col, int row)
    {
        return row >= 0 && row <= 7 && col >= 0 && col <= 7;
    }

    public static int fromColRow(int col, int row) => row * 8 + col;

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

public static class BoardPositionExtensions
{
    public static int getCol(this int index) => index % 8;
    public static int getRow(this int index) => index / 8;
    public static BoardPosition toBoardPosition(this int index) => new BoardPosition(index);
}