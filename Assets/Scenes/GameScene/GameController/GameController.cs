using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState gameState = new GameState();
    public GameType gameType;
    private BoardController boardController;
    private PremoveQueue premoveQueue;

    public void Start()
    {
        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        premoveQueue = GameObject.Find(nameof(PremoveQueue)).GetComponent<PremoveQueue>();
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

    public void PlayAnimatedMove(Move move, bool animated)
    {
        var isValidMove = gameState.getLegalMoves().Any(legalMove => legalMove.Equals(move));

        if (!isValidMove)
        {
            if (!IsPremoveMode())
            {
                premoveQueue.Clear();
                boardController.ResetPieces(gameState);
                return;
            }

            premoveQueue.Push(move);
            boardController.MakeSimpleMove(move);
            return;
        }

        gameState.PlayMove(move);
        boardController.AnimateMove(move, animated);
        if (gameType == GameType.HumanHuman) boardController.RotateBoard();
        TriggerAiMoveIfNeeded();
        PopPremoveQueueIfNeeded();
    }

    public bool MoveResultsInPromotion(Move move)
    {
        return gameState.MoveResultsInPromotion(move);
    }

    public bool IsPremoveMode()
    {
        if (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn) return true;
        if (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn) return true;
        return false;
    }

    private async void TriggerAiMoveIfNeeded()
    {
        if (gameState.getLegalMoves().Count == 0) return;

        if (gameType == GameType.Ai1WhiteAi2Black
        || gameType == GameType.Ai1BlackAi2White
        || (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn)
        || (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn))
        {
            var aiMove = await GameObject.Find(nameof(AiController)).GetComponent<AiController>().GetMove(gameState);
            PlayAnimatedMove(aiMove, true);
        }
    }

    private void PopPremoveQueueIfNeeded()
    {
        if (gameType == GameType.Ai1WhiteAi2Black
        || gameType == GameType.Ai1BlackAi2White
        || gameType == GameType.HumanHuman) return;

        if (gameType == GameType.HumanWhiteAiBlack && !gameState.whiteTurn) return;
        if (gameType == GameType.HumanBlackAiWhite && gameState.whiteTurn) return;

        if (!premoveQueue.HasMoves()) return;

        boardController.ResetPieces(gameState);

        var premove = premoveQueue.Pop();

        PlayAnimatedMove(premove, false);
        foreach (var nextPremove in premoveQueue.GetMoves())
        {
            boardController.MakeSimpleMove(nextPremove);
        }
    }
}
