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
    private CancellationTokenSource cancellationToken;
    private System.Random random = new System.Random();
    public Task<Move> GetMove(GameStateInterface referenceGameState, TimeSpan remainingTime, TimeSpan increment)
    {
        cancellationToken = new CancellationTokenSource();
        var timeManagement = new Ai8TimeManagement(remainingTime, increment, cancellationToken.Token, ForceDepth);

        var gameState = new V14GameState(referenceGameState);
        var legalMoves = gameState.getLegalMoves();

        if (legalMoves.Count == 1) return Task.FromResult(legalMoves[0]);

        var depth = 1;
        var bestIndicesEver = Enumerable.Range(0, legalMoves.Count).ToList();
        var bestValueEver = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
        int lastCurrentMoveIndex;
        var nodesVisited = 0L;

        while (true)
        {
            var bestIndices = new List<int> { };
            var bestValue = gameState.boardState.WhiteTurn ? double.MinValue : double.MaxValue;
            var allTerminalLeaves = true;

            for (lastCurrentMoveIndex = 0; lastCurrentMoveIndex < legalMoves.Count; ++lastCurrentMoveIndex)
            {
                gameState.PlayMove(legalMoves[lastCurrentMoveIndex]);
                var searchResult = Ai8Search.Search(gameState, depth, timeManagement);
                nodesVisited += searchResult.nodeCount;
                gameState.UndoMove();

                if (timeManagement.ShouldStop(depth)) break;

                allTerminalLeaves = allTerminalLeaves && searchResult.terminalLeaf;

                if (searchResult.value == bestValue)
                {
                    bestIndices.Add(lastCurrentMoveIndex);
                }
                else if (searchResult.value > bestValue && gameState.boardState.WhiteTurn || searchResult.value < bestValue && !gameState.boardState.WhiteTurn)
                {
                    bestIndices = new List<int> { lastCurrentMoveIndex };
                    bestValue = searchResult.value;
                }
            }

            if (lastCurrentMoveIndex < legalMoves.Count) break;

            bestIndicesEver = bestIndices;
            bestValueEver = bestValue;
            ++depth;
            lastCurrentMoveIndex = 0;

            // Don't go deeper if check mate can be delivered at searched depth
            if (bestValue == double.MaxValue && gameState.boardState.WhiteTurn || bestValue == double.MinValue && !gameState.boardState.WhiteTurn)
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
            Debug.Log($"Ai8 Depth: {depth}, ratio: {lastCurrentMoveIndex}/{legalMoves.Count}, Nodes: {nodesVisited}, Time: {timeManagement.GetElapsed().TotalSeconds:0.000}/{remainingTime.TotalSeconds:0.000}, Best moves: {bestIndicesEver.Count}, Evaluation: {bestValueEver}");
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
        if (cancellationToken != null)
        {
            cancellationToken.Cancel();
            cancellationToken = null;
        }
    }
}
