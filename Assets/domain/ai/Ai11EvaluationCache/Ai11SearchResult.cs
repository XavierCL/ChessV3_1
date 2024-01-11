public class Ai11SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;
  public readonly double integral;
  public readonly int gameTurn;
  public readonly int depth;

  public Ai11SearchResult(double value, bool terminalLeaf, long nodeCount, double integral, int gameTurn, int depth)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    this.integral = integral;
    this.gameTurn = gameTurn;
    this.depth = depth;
  }

  public Ai11SearchResult(Ai11SearchResult idle, Ai11SearchResult child, long nodeCount, int depth)
  {
    this.value = child.value;
    this.terminalLeaf = child.terminalLeaf;
    this.nodeCount = nodeCount;
    var turnDiff = child.gameTurn - idle.gameTurn;
    this.integral = idle.value / (turnDiff + 1) + turnDiff * (child.integral / (turnDiff + 1));
    this.gameTurn = child.gameTurn;
    this.depth = depth;
  }

  public Ai11SearchResult(Ai11SearchResult idle, Ai11SearchResult child, bool terminalLeaf, long nodeCount, int depth)
  {
    this.value = child.value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    var turnDiff = child.gameTurn - idle.gameTurn;
    this.integral = idle.value / (turnDiff + 1) + turnDiff * (child.integral / (turnDiff + 1));
    this.gameTurn = child.gameTurn;
    this.depth = depth;
  }

  public static Ai11SearchResult FromDraw(int gameTurn)
  {
    return new Ai11SearchResult(0, true, 1, 0, gameTurn, 1);
  }

  public Ai11SearchResult SetValue(double value)
  {
    return new Ai11SearchResult(value, terminalLeaf, nodeCount, value, gameTurn, depth);
  }

  public Ai11SearchResult ResetGameTurn(int idleGameTurn)
  {
    return new Ai11SearchResult(value, terminalLeaf, nodeCount, integral, idleGameTurn + depth, depth);
  }

  public bool IsBetterThan(Ai11SearchResult other, V14GameState gameState)
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

  public bool IsTheSameAs(Ai11SearchResult other)
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