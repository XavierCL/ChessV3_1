
using UnityEngine;

public class AiOnlyGameVisual : GameVisual
{
    private bool whiteTop;

    public AiOnlyGameVisual(bool whiteTop)
    {
        this.whiteTop = whiteTop;
    }

    public override void StartGame(GameState gameState)
    {
        if (whiteTop) boardController.RotateBlackBottom();
        clocks.Restart(whiteTop);
        base.StartGame(gameState);
    }

    public override void Cleanup()
    {
        if (whiteTop) boardController.RotateWhiteBottom();
        base.Cleanup();
    }

    public override void BoardMousePress()
    { }

    public override void BoardMouseRelease()
    { }
}
