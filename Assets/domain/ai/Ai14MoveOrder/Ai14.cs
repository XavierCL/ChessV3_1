using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Ai14 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    public bool DontStartNextDepthAfterHalfTime = true;
    public bool InitialRandomOrder = true;
    public bool searchExtensions = true;

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
            while (ownGameState.history.Count < referenceGameState.history.Count)
            {
                ownGameState.PlayMove(new Move(referenceGameState.history[^(referenceGameState.history.Count - ownGameState.history.Count)]));
            }
        }

        var gameState = ownGameState;
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1)
        {
            if (ShowDebugInfo)
            {
                Debug.Log($"Ai14 One legal move");
            }
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var timeManagement = new Ai14TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);
        var hyperParameters = new Ai14SearchResult.HyperParameters(timeManagement, searchExtensions);
        var bestIndexEver = 0;
        var rootAlpha = new Ai14SearchResult(gameState.boardState.WhiteTurn ? double.MaxValue : double.MinValue, false, 0);
        var rootBeta = new Ai14SearchResult(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue, false, 0);
        var bestResultEver = rootBeta;
        int lastCurrentMoveIndex = 0;
        var nodesVisited = 1L;
        var bestMoveOrder = Enumerable.Range(0, legalMoves.Count).ToArray();

        if (InitialRandomOrder)
        {
            bestMoveOrder = bestMoveOrder.OrderBy(_moveIndex => random.Next()).ToArray();
        }

        var allPieces = 0ul;

        for (var bitBoardIndex = 0; bitBoardIndex < 12; ++bitBoardIndex)
        {
            allPieces |= gameState.boardState.bitBoards[bitBoardIndex];
        }

        bestMoveOrder = bestMoveOrder.OrderBy(moveIndex => gameState.boardState.whiteTurn ? -Ai14Evaluate.EvaluateMove(legalMoves[moveIndex], gameState, allPieces) : Ai14Evaluate.EvaluateMove(legalMoves[moveIndex], gameState, allPieces)).ToArray();

        while (true)
        {
            var bestIndex = 0;
            var bestSearchResult = rootBeta;
            var allTerminalLeaves = true;
            var searchResults = new Ai14SearchResult[legalMoves.Count];

            if (timeManagement.ShouldStop(depth, DontStartNextDepthAfterHalfTime)) break;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                var moveIndex = bestMoveOrder[lastCurrentMoveIndex];

                gameState.PlayMove(legalMoves[moveIndex]);
                var searchResult = Ai14Search.Search(gameState, depth, bestSearchResult, rootAlpha, hyperParameters);
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

            // Time management stopped the current iteration, don't register the partial results.
            if (lastCurrentMoveIndex < legalMoves.Count) break;

            bestIndexEver = bestIndex;
            bestResultEver = bestSearchResult;
            ++depth;
            lastCurrentMoveIndex = 0;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestSearchResult.IsBestTerminal(gameState)) break;

            // Don't go deeper if the tree has reached only terminal leaves.
            if (allTerminalLeaves) break;

            // Refine next depth search order, so alpha beta works better
            bestMoveOrder = bestMoveOrder.OrderBy(order => searchResults[order], new Ai14SearchResult.Comparer(gameState)).ToArray();
        }

        if (ShowDebugInfo)
        {
            Debug.Log($"Ai14 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, {bestResultEver}");
        }

        averageUsefulDepth = (averageUsefulDepth * moveCount + (depth - 1)) / (moveCount + 1);
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
