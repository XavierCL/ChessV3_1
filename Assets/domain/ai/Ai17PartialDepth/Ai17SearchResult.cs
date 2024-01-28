using System;
using System.Collections;
using System.Collections.Generic;

public class Ai17SearchResult
{
  public readonly double value;
  public readonly bool terminalLeaf;
  public readonly long nodeCount;

  public Ai17SearchResult(double value, bool terminalLeaf, long nodeCount)
  {
    this.value = value;
    this.terminalLeaf = terminalLeaf;
    this.nodeCount = nodeCount;
  }

  public Ai17SearchResult SetParentSearch(bool allTerminal, long nodeCount)
  {
    return new Ai17SearchResult(value, allTerminal, nodeCount);
  }

  public bool IsBetterThan(Ai17SearchResult other, V17GameState gameState)
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

  public bool IsTheSameAs(Ai17SearchResult other)
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

  public class Comparer : IComparer<Ai17SearchResult>
  {
    private readonly V17GameState gameState;

    public Comparer(V17GameState gameState)
    {
      this.gameState = gameState;
    }

    public int Compare(Ai17SearchResult a, Ai17SearchResult b)
    {
      return Math.Sign(gameState.boardState.whiteTurn ? b.value - a.value : a.value - b.value);
    }
  }

  public class HyperParameters
  {
    public readonly Ai17TimeManagement timeManagement;
    public readonly bool searchExtensions;
    public readonly int sortFromDepth;
    public readonly double middleKingEndGame;

    public HyperParameters(Ai17TimeManagement timeManagement, bool searchExtensions, int sortFromDepth, double middleKingEndGame)
    {
      this.timeManagement = timeManagement;
      this.searchExtensions = searchExtensions;
      this.sortFromDepth = sortFromDepth;
      this.middleKingEndGame = middleKingEndGame;
    }
  }
}