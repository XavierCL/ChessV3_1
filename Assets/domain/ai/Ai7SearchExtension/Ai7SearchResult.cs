public class Ai7SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai7SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }
}