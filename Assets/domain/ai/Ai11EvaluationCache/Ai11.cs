using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai11 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    public double AcceptableDelta = 0.005;
    public double Temperature = 3.0;
    public bool ShowAllAcceptableMoves = false;
    public bool ShowCacheInfo = false;

    private readonly System.Random random = new System.Random();

    private CancellationTokenSource cancellationToken;
    private V14GameState ownGameState;
    private double averageUsefulDepth = 0.0;
    private double averageAcceptableMoves = 0.0;
    private int moveCount = 0;
    private MapCache<V14BoardState, Ai11SearchResult> evaluationCache;

    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        var currentCancellationToken = new CancellationTokenSource();
        cancellationToken = currentCancellationToken;
        var timeManagement = new Ai11TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

        if (ownGameState == null)
        {
            ownGameState = new V14GameState(referenceGameState);
            evaluationCache = new MapCache<V14BoardState, Ai11SearchResult>(999_983);
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
                Debug.Log($"Ai11 One legal move");
            }
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var allSearchResultsEver = new List<Ai11SearchResult>();
        int lastCurrentMoveIndex = 0;
        var nodesVisited = 1L;
        var orderedMoveIndices = new List<int>();

        while (true)
        {
            var searchResults = new List<Ai11SearchResult>(legalMoves.Count);

            if (timeManagement.ShouldStop(depth)) break;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                gameState.PlayMove(legalMoves[lastCurrentMoveIndex]);
                var searchResult = Ai11Search.Search(gameState, depth, legalMoves.Count, legalMoves.Count, evaluationCache, timeManagement);
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

            var allTerminal = allSearchResultsEver.All(searchResult => searchResult.terminalLeaf);
            var cacheEntry = evaluationCache.Get(gameState.boardState);
            var idleEvaluation = cacheEntry != null ? cacheEntry.ResetGameTurn(gameState.History.Count) : Ai11Evaluate.Evaluate(gameState, 0, 0);
            orderedMoveIndices = Enumerable.Range(0, allSearchResultsEver.Count).ToList();
            orderedMoveIndices.Sort((a, b) => allSearchResultsEver[a].IsBetterThan(allSearchResultsEver[b], gameState) ? -1 : 1);
            evaluationCache.Set(gameState.boardState, new Ai11SearchResult(idleEvaluation, allSearchResultsEver[orderedMoveIndices[0]], allTerminal, gameState.History.Count, depth));

            // Don't go deeper if check mate can be delivered at searched depth
            if (allSearchResultsEver[^1].IsBestTerminal(gameState)) break;

            // Don't go deeper if the tree has reached only terminal leaves.
            if (allTerminal) break;
        }

        if (allSearchResultsEver.Count == 0)
        {
            throw new Exception("Cannot set legal move index to play");
        }

        var bestValue = allSearchResultsEver[orderedMoveIndices[0]].value;

        orderedMoveIndices = orderedMoveIndices
            .Where(moveIndex => gameState.boardState.whiteTurn
                ? allSearchResultsEver[moveIndex].value >= bestValue - AcceptableDelta
                : allSearchResultsEver[moveIndex].value <= bestValue + AcceptableDelta
            )
            .ToList();

        var randomValue = random.NextDouble();
        var randomIndex = (int)Math.Floor(Math.Pow(randomValue, Temperature) * orderedMoveIndices.Count);

        if (ShowDebugInfo)
        {
            if (ShowAllAcceptableMoves)
            {
                var reversedMoves = new List<int>(orderedMoveIndices);
                reversedMoves.Reverse();
                foreach (var orderedMoveIndex in reversedMoves)
                {
                    var legalMove = legalMoves[orderedMoveIndex];
                    var searchResult = allSearchResultsEver[orderedMoveIndex];
                    Debug.Log($"Move: {legalMoves[orderedMoveIndex]}, {allSearchResultsEver[orderedMoveIndex]}");
                }

                Debug.Log($"Chosen move: {legalMoves[orderedMoveIndices[randomIndex]]}");
            }

            Debug.Log($"Ai11 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, Best moves: {orderedMoveIndices.Count}, {allSearchResultsEver[orderedMoveIndices[randomIndex]]}");
        }

        if (ShowCacheInfo)
        {
            Debug.Log($"Ai11 Cache: {evaluationCache}");
        }

        averageUsefulDepth = (averageUsefulDepth * moveCount + (depth - 1)) / (moveCount + 1);
        averageAcceptableMoves = (averageAcceptableMoves * moveCount + orderedMoveIndices.Count) / (moveCount + 1);
        ++moveCount;

        cancellationToken = null;

        return Task.FromResult(legalMoves[orderedMoveIndices[randomIndex]]);
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
        return $"Average depth: {averageUsefulDepth:0.00}, Average acceptable moves: {averageAcceptableMoves:0.00}";
    }

    public void ResetStats()
    {
        averageUsefulDepth = 0;
        averageAcceptableMoves = 0;
        moveCount = 0;
    }
}
