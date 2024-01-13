using System.Diagnostics;

[DebuggerDisplay("{value}")]
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

  public bool IsBetterThan(Ai8SearchResult other, V14GameState gameState)
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

  public bool IsBestTerminal(V14GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public bool IsTheSameAs(Ai8SearchResult other)
  {
    return value == other.value;
  }

  public Ai8SearchResult SetParentSearch(bool allTerminal, long nodeCount)
  {
    return new Ai8SearchResult(value, allTerminal, nodeCount);
  }

  public class Hyperparameters
  {
    public readonly Ai8TimeManagement timeManagement;
    public readonly bool searchExtensions;

    public Hyperparameters(Ai8TimeManagement timeManagement, bool searchExtensions)
    {
      this.timeManagement = timeManagement;
      this.searchExtensions = searchExtensions;
    }
  }
}