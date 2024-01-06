using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Ai10 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    public double AcceptableDelta = 0.01;
    public bool ShowAllAcceptableMoves = false;
    private CancellationTokenSource cancellationToken;
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai10TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

        var gameState = new V14GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1)
        {
            Debug.Log($"Ai10 One legal move");
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var allSearchResultsEver = new List<Ai10SearchResult>();
        int lastCurrentMoveIndex;
        var nodesVisited = 1L;

        while (true)
        {
            var searchResults = new List<Ai10SearchResult>(legalMoves.Count);

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                gameState.PlayMove(legalMoves[lastCurrentMoveIndex]);
                var nextIdle = Ai10Evaluate.Evaluate(gameState, legalMoves.Count);
                var searchResult = Ai10Search.Search(gameState, depth, nextIdle, timeManagement);
                nodesVisited += searchResult.nodeCount;
                gameState.UndoMove();

                if (timeManagement.ShouldStop(depth)) break;

                searchResults.Add(searchResult);

                if (searchResult.IsBestTerminal(gameState)) break;
            }

            if (timeManagement.ShouldStop(depth)) break;

            allSearchResultsEver = searchResults;
            ++depth;
            lastCurrentMoveIndex = 0;

            // Don't go deeper if check mate can be delivered at searched depth
            if (allSearchResultsEver[^1].IsBestTerminal(gameState)) break;

            // Don't go deeper if the tree has reached only terminal leaves.
            if (allSearchResultsEver.All(searchResult => searchResult.terminalLeaf)) break;
        }

        if (allSearchResultsEver.Count == 0)
        {
            throw new Exception("Cannot set legal move index to play");
        }

        var orderedMoveIndices = Enumerable.Range(0, allSearchResultsEver.Count).ToList();
        orderedMoveIndices.Sort((a, b) => allSearchResultsEver[a].IsBetterThan(allSearchResultsEver[b], gameState) ? -1 : 1);
        var bestValue = allSearchResultsEver[orderedMoveIndices[0]].value;

        orderedMoveIndices = orderedMoveIndices
            .Where(moveIndex => gameState.boardState.whiteTurn
                ? allSearchResultsEver[moveIndex].value >= bestValue - AcceptableDelta
                : allSearchResultsEver[moveIndex].value <= bestValue + AcceptableDelta
            )
            .ToList();

        var randomValue = random.NextDouble();
        var randomIndex = (int)Math.Floor(randomValue * randomValue * randomValue * orderedMoveIndices.Count);

        if (ShowDebugInfo)
        {
            if (ShowAllAcceptableMoves)
            {
                var reveredMoves = new List<int>(orderedMoveIndices);
                reveredMoves.Reverse();
                foreach (var orderedMoveIndex in reveredMoves)
                {
                    var legalMove = legalMoves[orderedMoveIndex];
                    var searchResult = allSearchResultsEver[orderedMoveIndex];
                    Debug.Log($"Move: {legalMoves[orderedMoveIndex]}, {allSearchResultsEver[orderedMoveIndex]}");
                }

                Debug.Log($"Chosen move: {legalMoves[orderedMoveIndices[randomIndex]]}");
            }

            Debug.Log($"Ai10 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, {allSearchResultsEver[orderedMoveIndices[randomIndex]]}");
        }

        cancellationToken = null;

        return Task.FromResult(legalMoves[orderedMoveIndices[randomIndex]]);
    }

    public void ResetAi()
    {
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken = null;
        }
    }
}
