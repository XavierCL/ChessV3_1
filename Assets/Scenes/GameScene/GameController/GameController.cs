using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameState gameState = new GameState();
    public GameType gameType;
    public GameEndState gameEndState = GameEndState.Ongoing;
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
        this.gameEndState = GameEndState.Ongoing;
        gameState = new GameState();

        // Handle board rotation
        switch (this.gameType)
        {
            case GameType.HumanHuman:
            case GameType.HumanWhiteAiBlack:
            case GameType.Ai1WhiteAi2Black:
                GetBoardController().RotateWhiteBottom();
                GameObject.Find(nameof(Clocks)).GetComponent<Clocks>().Restart(false);
                break;
            case GameType.HumanBlackAiWhite:
            case GameType.Ai1BlackAi2White:
                GetBoardController().RotateBlackBottom();
                GameObject.Find(nameof(Clocks)).GetComponent<Clocks>().Restart(true);
                break;
        }

        GetBoardController().ClearAnimations();
        GetBoardController().ResetPieces(gameState);
        GameObject.Find(nameof(AiController)).GetComponent<AiController>().ResetAis();
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
            GetBoardController().MakePremove(move);
            return;
        }

        GetBoardController().AnimateMove(move, animated, gameState);
        gameState.PlayMove(move);

        if (gameState.GetGameEndState() != GameEndState.Ongoing)
        {
            this.gameEndState = gameState.GetGameEndState();
            GetBoardController().ResetPieces(gameState);
            premoveQueue.Clear();
            GameObject.Find(nameof(Clocks)).GetComponent<Clocks>().Stop();
            return;
        }

        GameObject.Find(nameof(Clocks)).GetComponent<Clocks>().MovePlayed();
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

    public void NoMoreTime(bool topPlayer)
    {
        GameObject.Find(nameof(AiController)).GetComponent<AiController>().ResetAis();

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

        GetBoardController().ResetPieces(gameState);
        premoveQueue.Clear();
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

            // Coming back on another thread. If there's no move, then unity has stopped.
            if (aiMove == null) return;

            PlayAnimatedMove(aiMove.move, true);
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
            GetBoardController().MakePremove(nextPremove);
        }
    }

    private BoardController GetBoardController()
    {
        if (boardController != null) return boardController;

        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();

        return boardController;
    }
}
