using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiFighter : MonoBehaviour
{
    private AiController aiController;
    public string initialTime = "1";
    public string increment = "1";
    public bool OnlyFirstPosition = false;
    public bool ReversePositions = true;
    public bool SingleMove = false;

    void Awake()
    {
        aiController = GameObject.Find(nameof(AiController)).GetComponent<AiController>();
    }

    void Start()
    {
        LoopAis();
    }

    async void LoopAis()
    {
        var ai1Wins = 0;
        var ai2Wins = 0;
        var draws = 0;
        var lossByTime = 0;
        var increment = SingleClock.stringToTimeSpan(this.increment);
        var ai1Time = TimeSpan.Zero;
        var ai2Time = TimeSpan.Zero;
        var startingPositions = StartingPositions();
        var movePlayed = 0;
        var gameStateFactory = new V10GameStateFactory();

        if (OnlyFirstPosition)
        {
            startingPositions = new List<GameStateInterface> { startingPositions.First() };
        }

        for (var gameStateIndex = 0; gameStateIndex < startingPositions.Count; ++gameStateIndex)
        {
            var originalGameState = startingPositions[gameStateIndex];
            GameStateInterface ai1WhiteGameState = gameStateFactory.FromGameState(originalGameState);

            var ai1Clock = SingleClock.stringToTimeSpan(initialTime);
            var ai2Clock = ai1Clock;

            while (ai1WhiteGameState.GetGameEndState() == GameEndState.Ongoing)
            {
                var startTime = DateTime.UtcNow;

                var move = await aiController.GetMoveSync(
                    ai1WhiteGameState,
                    ai1WhiteGameState.BoardState.WhiteTurn,
                    ai1WhiteGameState.BoardState.WhiteTurn ? ai1Clock : ai2Clock,
                    increment
                );

                var elapsed = DateTime.UtcNow - startTime;
                ++movePlayed;
                if (ai1WhiteGameState.BoardState.WhiteTurn)
                {
                    ai1Time += elapsed;
                    ai1Clock -= elapsed;

                    if (ai1Clock <= TimeSpan.Zero) break;

                    ai1Clock += increment;
                }
                else
                {
                    ai2Time += elapsed;
                    ai2Clock -= elapsed;

                    if (ai2Clock <= TimeSpan.Zero) break;

                    ai2Clock += increment;
                }
                ai1WhiteGameState.PlayMove(move);

                if (originalGameState.BoardState.WhiteTurn == ai1WhiteGameState.BoardState.WhiteTurn && SingleMove) break;
            }

            if (ai1WhiteGameState.GetGameEndState() == GameEndState.Ongoing)
            {
                ++draws;
            }
            else if (ai1WhiteGameState.GetGameEndState() == GameEndState.Draw)
            {
                ++draws;
            }
            else if (ai1WhiteGameState.GetGameEndState() == GameEndState.WhiteWin || ai2Clock < TimeSpan.Zero)
            {
                if (ai2Clock < TimeSpan.Zero) ++lossByTime;
                ++ai1Wins;
            }
            else
            {
                if (ai1Clock < TimeSpan.Zero) ++lossByTime;
                ++ai2Wins;
            }

            if (!ReversePositions)
            {
                Debug.Log($"{gameStateIndex + 1}/{startingPositions.Count}");
                continue;
            }

            Debug.Log($"{gameStateIndex * 2 + 1}/{startingPositions.Count * 2}");

            ai1Clock = SingleClock.stringToTimeSpan(initialTime);
            ai2Clock = ai1Clock;
            GameStateInterface ai2WhiteGameState = gameStateFactory.FromGameState(originalGameState);

            while (ai2WhiteGameState.GetGameEndState() == GameEndState.Ongoing)
            {
                var startTime = DateTime.UtcNow;

                var move = await aiController.GetMoveSync(
                    ai2WhiteGameState,
                    !ai2WhiteGameState.BoardState.WhiteTurn,
                    ai2WhiteGameState.BoardState.WhiteTurn ? ai2Clock : ai1Clock,
                    increment
                );

                var elapsed = DateTime.UtcNow - startTime;
                ++movePlayed;
                if (ai2WhiteGameState.BoardState.WhiteTurn)
                {
                    ai2Time += elapsed;
                    ai2Clock -= elapsed;

                    if (ai2Clock <= TimeSpan.Zero) break;

                    ai2Clock += increment;
                }
                else
                {
                    ai1Time += elapsed;
                    ai1Clock -= elapsed;

                    if (ai1Clock <= TimeSpan.Zero) break;

                    ai1Clock += increment;
                }
                ai2WhiteGameState.PlayMove(move);

                if (originalGameState.BoardState.WhiteTurn == ai2WhiteGameState.BoardState.WhiteTurn && SingleMove) break;
            }

            Debug.Log($"{gameStateIndex * 2 + 2}/{startingPositions.Count * 2}");

            if (ai2WhiteGameState.GetGameEndState() == GameEndState.Ongoing)
            {
                ++draws;
            }
            else if (ai2WhiteGameState.GetGameEndState() == GameEndState.Draw)
            {
                ++draws;
            }
            else if (ai2WhiteGameState.GetGameEndState() == GameEndState.WhiteWin || ai1Clock <= TimeSpan.Zero)
            {
                if (ai1Clock < TimeSpan.Zero) ++lossByTime;
                ++ai2Wins;
            }
            else
            {
                if (ai2Clock < TimeSpan.Zero) ++lossByTime;
                ++ai1Wins;
            }
        }

        Debug.Log("Ai1 wins: " + ai1Wins);
        Debug.Log("Ai2 wins: " + ai2Wins);
        Debug.Log("Draws: " + draws);
        Debug.Log("Ai1 time: " + ai1Time.TotalSeconds);
        Debug.Log("Ai2 time: " + ai2Time.TotalSeconds);
        Debug.Log("Move played: " + movePlayed);
    }

    private List<GameStateInterface> StartingPositions()
    {
        return new List<GameStateInterface> {
            new V9GameStateFactory().StartingPosition(),
            new V9GameStateFactory().FromFen("rnbqkb1r/pppp1p1p/5np1/4p3/2B1P3/2N2N2/PPPP1PPP/R1BQK2R b KQkq"),
            new V9GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP1p/RNB1K1R1 w -"),
            new V9GameStateFactory().FromFen("2kr1b2/1bp4r/p1nq1p2/3pp3/P3n1P1/3P4/1PP1QP2/RNB2KRr w -"),
            new V9GameStateFactory().FromFen("1r6/4b3/p6P/k7/2p5/8/5r2/7K b -"),
            new V9GameStateFactory().FromFen("rnbqk2r/pp2bpp1/5n1p/P2pp3/8/NP2PNPB/1pPPQP1P/R3K2R b KQkq"),
        };
    }
}
