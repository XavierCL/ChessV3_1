using System;
using System.Collections;
using System.Collections.Generic;

public class Ai14SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai14SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }

  public Ai14SearchResult SetParentSearch(bool allTerminal, long nodeCount)
  {
    return new Ai14SearchResult(value, allTerminal, nodeCount);
  }

  public bool IsBetterThan(Ai14SearchResult other, V16GameState gameState)
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

  public bool IsTheSameAs(Ai14SearchResult other)
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

  public class Comparer : IComparer<Ai14SearchResult>
  {
    private readonly V16GameState gameState;

    public Comparer(V16GameState gameState)
    {
      this.gameState = gameState;
    }

    public int Compare(Ai14SearchResult a, Ai14SearchResult b)
    {
      return Math.Sign(gameState.boardState.whiteTurn ? b.value - a.value : a.value - b.value);
    }
  }

  public class HyperParameters
  {
    public readonly Ai14TimeManagement timeManagement;
    public readonly bool searchExtensions;
    public readonly int sortFromDepth;

    public HyperParameters(Ai14TimeManagement timeManagement, bool searchExtensions, int sortFromDepth)
    {
      this.timeManagement = timeManagement;
      this.searchExtensions = searchExtensions;
      this.sortFromDepth = sortFromDepth;
    }
  }
}