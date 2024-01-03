public class Ai7SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;

  public Ai7SearchResult(double value, bool terminalLeaf)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
  }
}