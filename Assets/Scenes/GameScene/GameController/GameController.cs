using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState gameState = new GameState();
    public GameType gameType;
    private PromotionHandler promotionHandler;
    private BoardController boardController;
    private PremoveQueue premoveQueue;

    public void Start()
    {
        promotionHandler = GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>();
        premoveQueue = GameObject.Find(nameof(PremoveQueue)).GetComponent<PremoveQueue>();
        StartNewGame(GameType.HumanHuman);
    }

    [ContextMenu("Set initial game")]
    public void SetInitialGame()
    {
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
                GetBoardController().RotateWhiteBottom();
                break;
            case GameType.HumanBlackAiWhite:
            case GameType.Ai1BlackAi2White:
                GetBoardController().RotateBlackBottom();
                break;
        }

        GetBoardController().ResetPieces(gameState);
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
                GetBoardController().ResetPieces(gameState);
                promotionHandler.CancelPromotion();
                return;
            }

            premoveQueue.Push(move);
            GetBoardController().MakeSimpleMove(move);
            return;
        }

        gameState.PlayMove(move);
        GetBoardController().AnimateMove(move, animated);
        if (gameType == GameType.HumanHuman) GetBoardController().RotateBoard();
        TriggerAiMoveIfNeeded();
        PopPremoveQueueIfNeeded();
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

        GetBoardController().ResetPieces(gameState);

        var premove = premoveQueue.Pop();

        PlayAnimatedMove(premove, false);
        foreach (var nextPremove in premoveQueue.GetMoves())
        {
            GetBoardController().MakeSimpleMove(nextPremove);
        }
    }

    private BoardController GetBoardController()
    {
        if (boardController != null) return boardController;

        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();

        return boardController;
    }
}
