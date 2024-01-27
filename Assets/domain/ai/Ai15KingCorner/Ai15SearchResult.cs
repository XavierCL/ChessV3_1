using System;
using System.Collections;
using System.Collections.Generic;

public class Ai15SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai15SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }

  public Ai15SearchResult SetParentSearch(bool allTerminal, long nodeCount)
  {
    return new Ai15SearchResult(value, allTerminal, nodeCount);
  }

  public bool IsBetterThan(Ai15SearchResult other, V17GameState gameState)
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

  public bool IsTheSameAs(Ai15SearchResult other)
  {
    return value == other.value;
  }

  public bool IsBestTerminal(V17GameState gameState)
  {
    return value == double.MaxValue && gameState.boardState.WhiteTurn || value == double.MinValue && !gameState.boardState.WhiteTurn;
  }

  public override string ToString()
  {
    return $"Evaluation: {value}";
  }

  public class Comparer : IComparer<Ai15SearchResult>
  {
    private readonly V17GameState gameState;

    public Comparer(V17GameState gameState)
    {
      this.gameState = gameState;
    }

    public int Compare(Ai15SearchResult a, Ai15SearchResult b)
    {
      return Math.Sign(gameState.boardState.whiteTurn ? b.value - a.value : a.value - b.value);
    }
  }

  public class HyperParameters
  {
    public readonly Ai15TimeManagement timeManagement;
    public readonly bool searchExtensions;
    public readonly int sortFromDepth;
    public readonly double middleKingEndGame;

    public HyperParameters(Ai15TimeManagement timeManagement, bool searchExtensions, int sortFromDepth, double middleKingEndGame)
    {
      this.timeManagement = timeManagement;
      this.searchExtensions = searchExtensions;
      this.sortFromDepth = sortFromDepth;
      this.middleKingEndGame = middleKingEndGame;
    }
  }
}