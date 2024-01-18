using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai8 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    public bool SearchExtensions = true;

    private readonly System.Random random = new System.Random();

    private CancellationTokenSource cancellationToken;
    private V14GameState ownGameState;
    private double averageUsefulDepth = 0.0;
    private int moveCount = 0;
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();

        if (ownGameState == null)
        {
            ownGameState = new V14GameState(referenceGameState);
        }
        else
        {
            while (ownGameState.History.Count < referenceGameState.History.Count)
            {
                ownGameState.PlayMove(new Move(referenceGameState.History[^(referenceGameState.History.Count - ownGameState.History.Count)]));
            }
        }

        var gameState = ownGameState;
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1)
        {
            if (ShowDebugInfo)
            {
                Debug.Log($"Ai8 One legal move");
            }
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var timeManagement = new Ai8TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);
        var hyperParameters = new Ai8SearchResult.Hyperparameters(timeManagement, SearchExtensions);
        var bestIndicesEver = Enumerable.Range(0, legalMoves.Count).ToList();
        var lowestResult = new Ai8SearchResult(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue, false, 0);
        var bestResultEver = lowestResult;
        int lastCurrentMoveIndex = 0;
        var nodesVisited = 1L;

        while (true)
        {
            var bestIndices = new List<int> { };
            var bestSearchResult = lowestResult;
            var allTerminalLeaves = true;

            if (timeManagement.ShouldStop(depth)) break;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                gameState.PlayMove(legalMoves[lastCurrentMoveIndex]);
                var searchResult = Ai8Search.Search(gameState, depth, hyperParameters);
                nodesVisited += searchResult.nodeCount;
                gameState.UndoMove();

                if (timeManagement.ShouldStop(depth)) break;

                allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

                if (searchResult.IsBetterThan(bestSearchResult, gameState))
                {
                    bestIndices = new List<int> { lastCurrentMoveIndex };
                    bestSearchResult = searchResult;

                    if (searchResult.IsBestTerminal(gameState))
                    {
                        lastCurrentMoveIndex = legalMoves.Count;
                        break;
                    }
                }
                else if (searchResult.IsTheSameAs(bestSearchResult))
                {
                    bestIndices.Add(lastCurrentMoveIndex);
                }
            }

            if (lastCurrentMoveIndex < legalMoves.Count) break;

            bestIndicesEver = bestIndices;
            bestResultEver = bestSearchResult;
            ++depth;
            lastCurrentMoveIndex = 0;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestSearchResult.IsBestTerminal(gameState))
            {
                break;
            }

            // Don't go deeper if the tree has reached only terminal leaves.
            if (allTerminalLeaves)
            {
                break;
            }
        }

        if (ShowDebugInfo)
        {
            Debug.Log($"Ai8 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, Best moves: {bestIndicesEver.Count}, Evaluation: {bestResultEver.value}");
        }

        if (bestIndicesEver.Count == 0)
        {
            throw new Exception("Cannot set legal move index to play");
        }

        averageUsefulDepth = (averageUsefulDepth * moveCount + (depth - 1)) / (moveCount + 1);
        ++moveCount;

        cancellationToken = null;

        return Task.FromResult(legalMoves[bestIndicesEver[random.Next(0, bestIndicesEver.Count)]]);
    }

    public void ResetAi()
    {
        ownGameState = null;
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken = null;
        }
    }

    public string GetStats()
    {
        return $"Average depth: {averageUsefulDepth:0.00}";
    }

    public void ResetStats()
    {
        averageUsefulDepth = 0;
        moveCount = 0;
    }
}
