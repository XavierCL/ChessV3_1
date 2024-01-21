public class Ai13SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai13SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }

  public Ai13SearchResult SetParentSearch(bool allTerminal, long nodeCount)
  {
    return new Ai13SearchResult(value, allTerminal, nodeCount);
  }

  public bool IsBetterThan(Ai13SearchResult other, V16GameState gameState)
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

  public bool IsTheSameAs(Ai13SearchResult other)
  {
    return value == other.value;
  }

  public bool IsBestTerminal(V16GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public override string ToString()
  {
    return $"Evaluation: {value}";
  }

  public class Hyperparameters
  {
    public readonly Ai13TimeManagement timeManagement;
    public readonly bool searchExtensions;

    public Hyperparameters(Ai13TimeManagement timeManagement, bool searchExtensions)
    {
      this.timeManagement = timeManagement;
      this.searchExtensions = searchExtensions;
    }
  }
}