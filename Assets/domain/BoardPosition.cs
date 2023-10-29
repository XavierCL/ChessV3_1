public struct BoardPosition
{
    public char index { get; set; }

    public int col { get => index % 8; }
    public int row { get => index / 8; }

    public BoardPosition(int col, int row)
    {
        index = (char)(row * 8 + col);
    }
}
