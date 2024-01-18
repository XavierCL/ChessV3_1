using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Based on ai8
public class Ai12 : MonoBehaviour, AiInterface
{
    public bool ShowDebugInfo = false;
    public int ForceDepth = -1;
    public bool DontStartNextDepthAfterHalfTime = true;

    private readonly System.Random random = new System.Random();

    private CancellationTokenSource cancellationToken;
    private V14GameState ownGameState;
    private double averageUsefulDepth = 0.0;
    private int moveCount = 0;

    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai12TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

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
                Debug.Log($"Ai12 One legal move");
            }
            return Task.FromResult(legalMoves[0]);
        }

        var depth = 1;
        var bestIndexEver = 0;
        var rootAlpha = new Ai12SearchResult(gameState.boardState.WhiteTurn ? double.MaxValue : double.MinValue, false, 0);
        var rootBeta = new Ai12SearchResult(gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue, false, 0);
        var bestResultEver = rootBeta;
        int lastCurrentMoveIndex = 0;
        var nodesVisited = 1L;
        var rootMoveOrder = Enumerable.Range(0, legalMoves.Count).OrderBy(_moveIndex => random.Next()).ToList();

        while (true)
        {
            var bestIndex = 0;
            var bestSearchResult = rootBeta;
            var allTerminalLeaves = true;

            if (timeManagement.ShouldStop(depth, DontStartNextDepthAfterHalfTime)) break;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                var moveIndex = rootMoveOrder[lastCurrentMoveIndex];

                gameState.PlayMove(legalMoves[moveIndex]);
                var searchResult = Ai12Search.Search(gameState, depth, bestSearchResult, rootAlpha, timeManagement);
                nodesVisited += searchResult.nodeCount;
                gameState.UndoMove();

                if (timeManagement.ShouldStop(depth)) break;

                allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

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
        }

        if (ShowDebugInfo)
        {
            Debug.Log($"Ai12 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, {bestResultEver}");
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
