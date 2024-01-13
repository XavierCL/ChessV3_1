public class Ai12SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai12SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }

  public Ai12SearchResult SetParentSearch(bool allTerminal, long nodeCount)
  {
    return new Ai12SearchResult(value, allTerminal, nodeCount);
  }

  public bool IsBetterThan(Ai12SearchResult other, V14GameState gameState)
  {
    if (gameState.boardState.whiteTurn)
    {
      return value > other.value;
    }
    else
    {
      return value < other.value;
    }
  }

  public bool IsTheSameAs(Ai12SearchResult other)
  {
    return value == other.value;
  }

  public bool IsBestTerminal(V14GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public override string ToString()
  {
    return $"Evaluation: {value}";
  }
}