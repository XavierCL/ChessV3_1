using UnityEngine;

public class HumanOnlyGameVisual : GameVisual
{
    private PromotionHandler promotionHandler;

    public HumanOnlyGameVisual()
    {
        promotionHandler = GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>();
    }

    public override void StartGame(GameState gameState)
    {
        base.StartGame(gameState);
        clocks.Restart(false);
    }

    public override void BoardMousePress(GameObject collision)
    {
        if (promotionHandler.PromotionInProgress)
        {
            var move = promotionHandler.FinalizePromotion(collision);
            if (move == null) return;

            gameController.PlayMove(move, true);
        }

        if (collision == null)
        {
            return;
        }

        // todo missing promotion prompt and selected piece
    }

    public override void BoardMouseRelease()
    {
        throw new System.NotImplementedException();
    }
}
