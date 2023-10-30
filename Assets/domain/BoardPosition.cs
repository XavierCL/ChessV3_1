public struct BoardPosition
{
    public int index { get; set; }

    public int col { get => index % 8; }
    public int row { get => index / 8; }

    public BoardPosition(int col, int row)
    {
        index = row * 8 + col;
    }
}
