using UnityEngine;

public abstract class GameVisual
{
    protected GameController gameController;
    protected BoardController boardController;
    protected Clocks clocks;

    public GameVisual()
    {
        gameController = GameObject.Find(nameof(GameController)).GetComponent<GameController>();
        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        clocks = GameObject.Find(nameof(Clocks)).GetComponent<Clocks>();
    }

    public virtual void StartGame(GameState gameState)
    {
        boardController.ResetPieces(gameState);
    }

    public virtual void StopGame(GameState gameState)
    {
        boardController.ResetPieces(gameState);
    }

    public virtual void Cleanup()
    {
        boardController.ClearAnimations();
    }

    public abstract void BoardMousePress(GameObject collision);
    public abstract void BoardMouseRelease();

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
}
