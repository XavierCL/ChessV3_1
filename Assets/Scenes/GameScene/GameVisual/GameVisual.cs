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

    public virtual void GameOver(GameState gameState)
    {
        boardController.ResetPieces(gameState);
        clocks.Stop();
    }

    public virtual void Cleanup()
    {
        boardController.ClearAnimations();
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
