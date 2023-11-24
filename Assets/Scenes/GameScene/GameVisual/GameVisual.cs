using UnityEngine;

public abstract class GameVisual
{
    protected GameController gameController;
    protected BoardController boardController;
    protected Clocks clocks;
    protected EndStateText endStateText;

    public GameVisual()
    {
        gameController = StaticReferences.gameController.Value;
        boardController = StaticReferences.boardController.Value;
        clocks = StaticReferences.clocks.Value;
        endStateText = StaticReferences.endStateText.Value;
    }

    public virtual void StartGame(GameStateInterface gameState)
    {
        boardController.ResetPieces(gameState);
        endStateText.SetEndState(gameState.GetGameEndState());
    }

    public virtual void GameOver(GameStateInterface gameState)
    {
        boardController.ResetPieces(gameState);
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
        boardController.AnimateMove(move, false, animated);
        clocks.MovePlayed();
    }

    public virtual void Update() { }
}
