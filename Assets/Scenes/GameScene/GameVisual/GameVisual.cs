using UnityEngine;

public abstract class GameVisual
{
    protected GameController gameController;
    protected BoardController boardController;
    protected HistoryHandler historyHandler;
    protected Clocks clocks;
    protected EndStateText endStateText;

    public GameVisual()
    {
        gameController = StaticReferences.gameController.Value;
        boardController = StaticReferences.boardController.Value;
        clocks = StaticReferences.clocks.Value;
        endStateText = StaticReferences.endStateText.Value;
        historyHandler = StaticReferences.historyHandler.Value;
    }

    public virtual void StartGame(GameStateInterface gameState)
    {
        historyHandler.Start();
        boardController.ResetPieces(gameState.BoardState);
        endStateText.SetEndState(gameState.GetGameEndState());
    }

    public virtual void GameOver(GameStateInterface gameState)
    {
        boardController.ResetPieces(gameState.BoardState);
        clocks.Stop();
        endStateText.SetEndState(gameState.GetGameEndState());
    }

    public virtual void Cleanup()
    {
        boardController.ClearAnimations();
        clocks.ResetSwap();
    }

    public abstract void BoardMousePress();
    public abstract void BoardMouseRelease();

    public virtual void PlayAnimatedMove(Move move, bool animated = true)
    {
        historyHandler.ResetHistory();
        boardController.AnimateMove(move, false, animated);
        clocks.MovePlayed();
    }

    public virtual void HistoryBack() { }

    public virtual void Update() { }
}
