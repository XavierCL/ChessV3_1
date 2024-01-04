public class Ai8SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai8SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }
}