public class Ai9SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;
  public readonly double integral;
  public readonly int gameTurn;

  public Ai9SearchResult(double value, bool terminalLeaf, long nodeCount, double integral, int gameTurn)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    this.integral = integral;
    this.gameTurn = gameTurn;
  }

  public Ai9SearchResult(Ai9SearchResult idle, Ai9SearchResult child, long nodeCount)
  {
    this.value = child.value;
    this.terminalLeaf = child.terminalLeaf;
    this.nodeCount = nodeCount;
    var turnDiff = child.gameTurn - idle.gameTurn;
    this.integral = idle.value / (turnDiff + 1) + turnDiff * (child.integral / (turnDiff + 1));
    this.gameTurn = child.gameTurn;
  }

  public Ai9SearchResult(Ai9SearchResult idle, Ai9SearchResult child, bool terminalLeaf, long nodeCount)
  {
    this.value = child.value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    var turnDiff = child.gameTurn - idle.gameTurn;
    this.integral = idle.value / (turnDiff + 1) + turnDiff * (child.integral / (turnDiff + 1));
    this.gameTurn = child.gameTurn;
  }

  public Ai9SearchResult SetValue(double value)
  {
    return new Ai9SearchResult(value, terminalLeaf, nodeCount, value, gameTurn);
  }

  public bool IsBetterThan(Ai9SearchResult other, V14GameState gameState)
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

  public bool IsTheSameAs(Ai9SearchResult other)
  {
    return value == other.value && integral == other.integral;
  }

  public bool IsBestTerminal(V14GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public override string ToString()
  {
    return $"Integral: {integral}, Evaluation: {value}";
  }
}