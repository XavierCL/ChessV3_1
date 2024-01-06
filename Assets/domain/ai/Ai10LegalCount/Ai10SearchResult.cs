public class Ai10SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;
  public readonly double integral;
  public readonly int gameTurn;

  public Ai10SearchResult(double value, bool terminalLeaf, long nodeCount, double integral, int gameTurn)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    this.integral = integral;
    this.gameTurn = gameTurn;
  }

  public Ai10SearchResult(Ai10SearchResult idle, Ai10SearchResult child, long nodeCount)
  {
    this.value = child.value;
    this.terminalLeaf = child.terminalLeaf;
    this.nodeCount = nodeCount;
    var turnDiff = child.gameTurn - idle.gameTurn;
    this.integral = idle.value / (turnDiff + 1) + turnDiff * (child.integral / (turnDiff + 1));
    this.gameTurn = child.gameTurn;
  }

  public Ai10SearchResult(Ai10SearchResult idle, Ai10SearchResult child, bool terminalLeaf, long nodeCount)
  {
    this.value = child.value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    var turnDiff = child.gameTurn - idle.gameTurn;
    this.integral = idle.value / (turnDiff + 1) + turnDiff * (child.integral / (turnDiff + 1));
    this.gameTurn = child.gameTurn;
  }

  public Ai10SearchResult SetValue(double value)
  {
    return new Ai10SearchResult(value, terminalLeaf, nodeCount, value, gameTurn);
  }

  public bool IsBetterThan(Ai10SearchResult other, V14GameState gameState)
  {
    if (gameState.boardState.whiteTurn)
    {
      return value > other.value || value == other.value && integral > other.integral;
    }
    else
    {
      return value < other.value || value == other.value && integral < other.integral;
    }
  }

  public bool IsTheSameAs(Ai10SearchResult other)
  {
    return value == other.value && integral == other.integral;
  }

  public bool IsBestTerminal(V14GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public override string ToString()
  {
    return $"Integral: {integral:0.000}, Evaluation: {value:0.000}";
  }
}