using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// from ai15
public class Ai17 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    public bool InitialRandomOrder = true;
    public bool DontStartNextDepthAfterHalfTime = true;
    public bool searchExtensions = true;
    public int sortFromDepth = 2;
    public double middleKingEndGame = 0.01;

    private readonly System.Random random = new System.Random();

    private CancellationTokenSource cancellationToken;
    private V17GameState ownGameState;
    private double averageUsefulDepth = 0.0;
    private int moveCount = 0;

    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();

        if (ownGameState == null)
        {
            ownGameState = new V17GameState(referenceGameState);
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
                Debug.Log($"Ai17 One legal move");
            }
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var timeManagement = new Ai17TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);
        var hyperParameters = new Ai17SearchResult.HyperParameters(timeManagement, searchExtensions, sortFromDepth, middleKingEndGame);
        var bestIndexEver = 0;
        var rootAlpha = new Ai17SearchResult(gameState.boardState.WhiteTurn ? double.MaxValue : double.MinValue, false, 0);
        var rootBeta = new Ai17SearchResult(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue, false, 0);
        var bestResultEver = rootBeta;
        int lastCurrentMoveIndex = 0;
        var nodesVisited = 1L;
        var bestMoveOrder = Enumerable.Range(0, legalMoves.Count).ToArray();

        if (InitialRandomOrder)
        {
            bestMoveOrder = bestMoveOrder.OrderBy(_moveIndex => random.Next()).ToArray();
        }

        while (true)
        {
            var bestIndex = bestIndexEver;
            var bestSearchResult = rootBeta;
            var allTerminalLeaves = true;
            var searchResults = new Ai17SearchResult[legalMoves.Count];

            if (timeManagement.ShouldStop(depth, DontStartNextDepthAfterHalfTime)) break;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                var moveIndex = bestMoveOrder[lastCurrentMoveIndex];

                gameState.PlayMove(legalMoves[moveIndex]);
                var searchResult = Ai17Search.Search(gameState, depth, bestSearchResult, rootAlpha, hyperParameters);
                nodesVisited += searchResult.nodeCount;
                gameState.UndoMove();

                if (timeManagement.ShouldStop(depth)) break;

                allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;
                searchResults[moveIndex] = searchResult;

                if (searchResult.IsBetterThan(bestSearchResult, gameState))
                {
                    bestIndex = moveIndex;
                    bestSearchResult = searchResult;

                    if (searchResult.IsBestTerminal(gameState))
                    {
                        lastCurrentMoveIndex = legalMoves.Count;
                        break;
                    }
                }
            }

            // Couldn't finish computing the first move, return early
            if (lastCurrentMoveIndex == 0) break;

            bestIndexEver = bestIndex;
            bestResultEver = bestSearchResult;

            // Time management ran out.
            if (lastCurrentMoveIndex < legalMoves.Count) break;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestSearchResult.IsBestTerminal(gameState)) break;

            // Don't go deeper if the tree has reached only terminal leaves.
            if (allTerminalLeaves) break;

            // Refine next depth search order, so alpha beta works better
            bestMoveOrder = bestMoveOrder.OrderBy(order => searchResults[order], new Ai17SearchResult.Comparer(gameState)).ToArray();

            lastCurrentMoveIndex = 0;
            ++depth;
        }

        if (ShowDebugInfo)
        {
            Debug.Log($"Ai17 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, {bestResultEver}");
        }

        var usefulDepth = depth + lastCurrentMoveIndex / (double)legalMoves.Count;
        averageUsefulDepth = (averageUsefulDepth * moveCount + (usefulDepth - 1)) / (moveCount + 1);
        ++moveCount;

        cancellationToken = null;

        return Task.FromResult(legalMoves[bestIndexEver]);
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
