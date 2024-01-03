public class Ai6SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;

  public Ai6SearchResult(double value, bool terminalLeaf)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
  }
}