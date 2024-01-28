using System;
using System.Collections.Generic;

public class Ai16SearchResult
{
  public readonly double value;
  public readonly double sum;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;
  public readonly bool alphaSkipped;

  public Ai16SearchResult(double value, double sum, bool terminalLeaf, long nodeCount, bool alphaSkipped)
  {
    this.value = value;
    this.sum = sum;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
    this.alphaSkipped = alphaSkipped;
  }

  public Ai16SearchResult SetParentSearch(double sum, bool allTerminal, long nodeCount)
  {
    return new Ai16SearchResult(value, sum, allTerminal, nodeCount, false);
  }

  public Ai16SearchResult SetAlphaSkiped(double sum, long nodeCount)
  {
    return new Ai16SearchResult(value, sum, false, nodeCount, true);
  }

  public bool IsBetterThan(Ai16SearchResult other, V17GameState gameState)
  {
    if (alphaSkipped != other.alphaSkipped) return other.alphaSkipped;

    if (gameState.boardState.whiteTurn)
    {
      return value > other.value || (value == other.value && sum > other.sum);
    }
    else
    {
      return value < other.value || (value == other.value && sum < other.sum);
    }
  }

  public bool IsBestTerminal(V17GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public override string ToString()
  {
    return $"Evaluation: {value}, Sum: {sum}";
  }

  public class Comparer : IComparer<Ai16SearchResult>
  {
    private readonly V17GameState gameState;

    public Comparer(V17GameState gameState)
    {
      this.gameState = gameState;
    }

    public int Compare(Ai16SearchResult a, Ai16SearchResult b)
    {
      if (a.alphaSkipped != b.alphaSkipped) return a.alphaSkipped ? 1 : -1;
      var valueSign = Math.Sign(gameState.boardState.whiteTurn ? b.value - a.value : a.value - b.value);
      if (valueSign != 0) return valueSign;
      return Math.Sign(gameState.boardState.whiteTurn ? b.sum - a.sum : a.sum - b.sum);
    }
  }

  public class HyperParameters
  {
    public readonly Ai16TimeManagement timeManagement;
    public readonly bool searchExtensions;
    public readonly int sortFromDepth;
    public readonly double middleKingEndGame;

    public HyperParameters(Ai16TimeManagement timeManagement, bool searchExtensions, int sortFromDepth, double middleKingEndGame)
    {
      this.timeManagement = timeManagement;
      this.searchExtensions = searchExtensions;
      this.sortFromDepth = sortFromDepth;
      this.middleKingEndGame = middleKingEndGame;
    }
  }
}