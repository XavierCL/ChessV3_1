using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState gameState = new GameState();
    public GameType gameType;
    public GameEndState gameEndState = GameEndState.Ongoing;
    public GameVisual gameVisual { get; private set; }
    private AiController aiController;

    public void Start()
    {
        StartNewGame(GameType.HumanHuman);
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
        gameState = new GameState();

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
                gameVisual = new AiOnlyGameVisual(true);
                break;
            case GameType.Ai1BlackAi2White:
                gameVisual = new AiOnlyGameVisual(false);
                break;
        }

        GetAiController().ResetAis();
        gameVisual.StartGame(gameState);
        TriggerAiMoveIfNeeded();
    }

    public void PlayMove(Move move)
    {
        // todo move most of this and player input handler into the game visual classes
        var isValidMove = gameState.getLegalMoves().Any(legalMove => legalMove.Equals(move));

        if (!isValidMove)
        {
            return;
        }

        gameState.PlayMove(move);

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
        if (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn) return true;
        if (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn) return true;
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

    private async void TriggerAiMoveIfNeeded()
    {
        if (gameState.getLegalMoves().Count == 0) return;

        if (gameType == GameType.Ai1WhiteAi2Black
        || gameType == GameType.Ai1BlackAi2White
        || (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn)
        || (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn))
        {
            var aiMove = await GetAiController().GetMove(gameState,
                (gameType == GameType.Ai1WhiteAi2Black) && gameState.whiteTurn ||
                (gameType == GameType.Ai1BlackAi2White) && !gameState.whiteTurn ||
                gameType == GameType.HumanWhiteAiBlack ||
                gameType == GameType.HumanBlackAiWhite
            );

            // Coming back on another thread. If there's no move, then unity has stopped.
            if (aiMove == null) return;

            PlayMove(aiMove);
        }
    }

    private AiController GetAiController()
    {
        if (aiController != null) return aiController;

        aiController = GameObject.Find(nameof(AiController)).GetComponent<AiController>();

        return aiController;
    }
}
