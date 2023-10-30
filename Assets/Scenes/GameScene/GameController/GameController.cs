using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState gameState = new GameState();
    public GameType gameType;
    private BoardController boardController;

    public void Start()
    {
        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        StartNewGame(GameType.HumanHuman);
    }

    public void StartNewGame(GameType gameType)
    {
        this.gameType = gameType;
        gameState = new GameState();

        // Handle board rotation
        switch (this.gameType)
        {
            case GameType.HumanHuman:
            case GameType.HumanWhiteAiBlack:
            case GameType.Ai1WhiteAi2Black:
                boardController.RotateWhiteBottom();
                break;
            case GameType.HumanBlackAiWhite:
            case GameType.Ai1BlackAi2White:
                boardController.RotateBlackBottom();
                break;
        }

        boardController.ResetPieces(gameState);
        TriggerAiMoveIfNeeded();
    }

    public void PlayImmediateMove(Move move)
    {
        gameState.PlayMove(move);

        if (gameType == GameType.HumanHuman) boardController.RotateBoard();
        TriggerAiMoveIfNeeded();
    }

    private void PlayAnimatedMove(Move move)
    {
        gameState.PlayMove(move);
        boardController.AnimateMove(move);
        TriggerAiMoveIfNeeded();
    }

    private async void TriggerAiMoveIfNeeded()
    {
        if (gameType == GameType.Ai1WhiteAi2Black
        || gameType == GameType.Ai1BlackAi2White
        || (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn)
        || (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn))
        {
            var aiMove = await GameObject.Find(nameof(AiController)).GetComponent<AiController>().GetMove(gameState);
            PlayAnimatedMove(aiMove);
        }
    }
}
