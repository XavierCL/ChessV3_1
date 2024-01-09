using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai9 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;

    private readonly System.Random random = new System.Random();

    private CancellationTokenSource cancellationToken;
    private V14GameState ownGameState;

    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai9TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

        if (ownGameState == null) {
            ownGameState = new V14GameState(referenceGameState);
        } else {
            while (ownGameState.history.Count < referenceGameState.history.Count) {
                ownGameState.PlayMove(new Move(referenceGameState.history[^(referenceGameState.history.Count - ownGameState.history.Count)]));
            }
        }

        var gameState = ownGameState;
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1)
        {
            if (ShowDebugInfo) {
                Debug.Log($"Ai9 One legal move");
            }
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var bestIndicesEver = Enumerable.Range(0, legalMoves.Count).ToList();
        var lowestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
        var bestValueEver = new Ai9SearchResult(lowestValue, false, 0, lowestValue, 0);
        int lastCurrentMoveIndex = 0;
        var nodesVisited = 1L;

        while (true)
        {
            var bestIndices = new List<int> { };
            var bestSearchResult = new Ai9SearchResult(lowestValue, false, 0, lowestValue, 0);
            var allTerminalLeaves = true;

            if (timeManagement.ShouldStop(depth)) break;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                gameState.PlayMove(legalMoves[lastCurrentMoveIndex]);
                var searchResult = Ai9Search.Search(gameState, depth, timeManagement);
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
            bestValueEver = bestSearchResult;
            ++depth;
            lastCurrentMoveIndex = 0;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestSearchResult.IsBestTerminal(gameState)) break;

            // Don't go deeper if the tree has reached only terminal leaves.
            if (allTerminalLeaves) break;
        }

        if (ShowDebugInfo)
        {
            Debug.Log($"Ai9 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, Best moves: {bestIndicesEver.Count}, {bestValueEver}");
        }

        if (bestIndicesEver.Count == 0)
        {
            throw new Exception("Cannot set legal move index to play");
        }

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
}
