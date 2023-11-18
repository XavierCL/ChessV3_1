using System;
using UnityEngine;

public class HumanAiGameVisual : GameVisual
{
    private bool humanWhite;
    private PremoveQueue premoveQueue;
    private PromotionHandler promotionHandler;

    public HumanAiGameVisual(bool humanWhite)
    {
        this.humanWhite = humanWhite;
        premoveQueue = GameObject.Find(nameof(PremoveQueue)).GetComponent<PremoveQueue>();
        promotionHandler = GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>();
    }

    public override void StartGame(GameState gameState)
    {
        if (!humanWhite) boardController.RotateBlackBottom();
        clocks.Restart(!humanWhite);
        base.StartGame(gameState);
    }

    public override void StopGame(GameState gameState)
    {
        premoveQueue.Clear();
        base.StopGame(gameState);
    }

    public override void Cleanup()
    {
        if (!humanWhite) boardController.RotateWhiteBottom();
        base.Cleanup();
    }

    public override void BoardMousePress(GameObject collision)
    {
        var isHumanTurn = humanWhite == gameController.gameState.whiteTurn;

        if (promotionHandler.PromotionInProgress)
        {
            var move = promotionHandler.FinalizePromotion(collision);

            if (move == null && isHumanTurn) return;

            if (move == null)
            {
                // Premoving, cancel premove
                premoveQueue.Clear();
                boardController.ResetPieces(gameController.gameState);
                promotionHandler.CancelPromotion();
                return;
            }

            if (isHumanTurn)
            {
                gameController.PlayMove(move, false);
                return;
            }

            // Premoving, add to queue
            premoveQueue.Push(move);
            boardController.MakePremove(move);
            return;
        }

        if (collision == null)
        {
            premoveQueue.Clear();
            boardController.ResetPieces(gameController.gameState);
            return;
        }

        // todo missing promotion prompt and selected piece
    }

    public override void BoardMouseRelease()
    {
        throw new System.NotImplementedException();
    }
}
