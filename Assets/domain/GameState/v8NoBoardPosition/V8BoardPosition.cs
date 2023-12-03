public static class V8BoardPosition
{
  public static int fromColRow(int col, int row) => row * 8 + col;
  public static int getCol(this int index) => index % 8;
  public static int getRow(this int index) => index % 8;
  public static BoardPosition toBoardPosition(this int index) => new BoardPosition(index);
}