using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool PrintFen = false;
    public GameStateFactoryInterface gameStateFactory;
    public GameStateInterface gameState;
    public GameType gameType;
    public GameEndState gameEndState = GameEndState.Ongoing;
    public GameVisual gameVisual { get; private set; }
    private AiController aiController;
    private SingleClock topClock;
    private SingleClock bottomClock;

    public GameController()
    {
        gameStateFactory = new PieceIdGameStateFactory();
        gameState = gameStateFactory.StartingPosition();
    }

    public void Start()
    {
        StartNewGame(Random.value >= 0.5 ? GameType.HumanWhiteAiBlack : GameType.HumanBlackAiWhite);
    }

    public void Update()
    {
        gameVisual?.Update();
    }

    [ContextMenu("Set initial game")]
    public void SetInitialGame()
    {
        StartNewGame(GameType.HumanHuman);
    }

    public void StartNewGame(GameType gameType)
    {
        gameVisual?.Cleanup();
        this.gameType = gameType;
        gameEndState = GameEndState.Ongoing;
        gameState = gameStateFactory.StartingPosition();

        switch (gameType)
        {
            case GameType.HumanHuman:
                gameVisual = new HumanOnlyGameVisual();
                break;
            case GameType.HumanWhiteAiBlack:
                gameVisual = new HumanAiGameVisual(true);
                break;
            case GameType.HumanBlackAiWhite:
                gameVisual = new HumanAiGameVisual(false);
                break;
            case GameType.Ai1WhiteAi2Black:
                gameVisual = new AiOnlyGameVisual(false);
                break;
            case GameType.Ai1BlackAi2White:
                gameVisual = new AiOnlyGameVisual(true);
                break;
        }

        GetAiController().ResetAis();
        gameVisual.StartGame(gameState);
        TriggerAiMoveIfNeeded();
    }

    public void PlayMove(Move move)
    {
        var isValidMove = gameState.getLegalMoves().Any(legalMove => legalMove.Equals(move));

        if (!isValidMove)
        {
            return;
        }

        gameState.PlayMove(move);

        if (PrintFen) Debug.Log(gameState.GetFen());

        gameEndState = gameState.GetGameEndState();

        gameVisual.PlayAnimatedMove(move);

        if (gameEndState != GameEndState.Ongoing)
        {
            gameVisual.GameOver(gameState);
            return;
        }

        TriggerAiMoveIfNeeded();
    }

    public bool IsPremoveMode()
    {
        if (gameType == GameType.HumanWhiteAiBlack && !gameState.BoardState.WhiteTurn) return true;
        if (gameType == GameType.HumanBlackAiWhite && gameState.BoardState.WhiteTurn) return true;
        return false;
    }

    public void NoMoreTime(bool topPlayer)
    {
        GetAiController().ResetAis();

        switch (gameType)
        {
            case GameType.HumanHuman:
            case GameType.HumanWhiteAiBlack:
            case GameType.Ai1WhiteAi2Black:
                gameEndState = topPlayer ? GameEndState.WhiteWin : GameEndState.BlackWin;
                break;
            case GameType.HumanBlackAiWhite:
            case GameType.Ai1BlackAi2White:
                gameEndState = topPlayer ? GameEndState.BlackWin : GameEndState.WhiteWin;
                break;
        }

        gameVisual.GameOver(gameState);
    }

    public bool IsAi1Turn(GameStateInterface gameState)
    {
        return (gameType == GameType.Ai1WhiteAi2Black) && gameState.BoardState.WhiteTurn ||
            (gameType == GameType.Ai1BlackAi2White) && !gameState.BoardState.WhiteTurn ||
            gameType == GameType.HumanWhiteAiBlack ||
            gameType == GameType.HumanBlackAiWhite;
    }

    private async void TriggerAiMoveIfNeeded()
    {
        if (gameState.getLegalMoves().Count == 0) return;

        if (gameType == GameType.Ai1WhiteAi2Black
        || gameType == GameType.Ai1BlackAi2White
        || (gameType == GameType.HumanWhiteAiBlack && !gameState.BoardState.WhiteTurn)
        || (gameType == GameType.HumanBlackAiWhite && gameState.BoardState.WhiteTurn))
        {
            var clock = IsAi1Turn(gameState) ? GetBottomClock() : GetTopClock();
            var aiMove = await GetAiController().GetMove(gameState, IsAi1Turn(gameState), clock.GetTimeLeft(), clock.GetIncrement());

            // Coming back on another thread. If there's no move, then unity has stopped.
            if (aiMove == null) return;

            PlayMove(aiMove);
        }
    }

    private AiController GetAiController()
    {
        if (aiController != null) return aiController;

        aiController = StaticReferences.aiController.Value;

        return aiController;
    }

    private SingleClock GetTopClock()
    {
        if (topClock != null) return topClock;

        topClock = StaticReferences.topClock.Value.GetComponent<SingleClock>();

        return topClock;
    }

    private SingleClock GetBottomClock()
    {
        if (bottomClock != null) return bottomClock;

        bottomClock = StaticReferences.bottomClock.Value.GetComponent<SingleClock>();

        return bottomClock;
    }
}
