using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PremoveHandler : MonoBehaviour
{
    public float SingleFrameZ = 0.01f;

    public Color PremoveTargetColor = Color.green;
    private GameController gameController;
    private BoardController boardController;
    private Shapes shapes;
    private PromotionHandler promotionHandler;
    private readonly Queue<Move> queue = new Queue<Move> { };

    void Awake()
    {
        gameController = StaticReferences.gameController.Value;
        boardController = StaticReferences.boardController.Value;
        promotionHandler = StaticReferences.promotionHandler.Value;
        shapes = StaticReferences.shapes.Value;
    }

    void Update()
    {
        if (promotionHandler.PromotionInProgress) return;

        foreach (var premove in GetMoves())
        {
            var premoveTargetWorldPosition = boardController.BoardPositionToWorldPosition(premove.target);
            shapes.Rectangle(new Vector3(premoveTargetWorldPosition.x, premoveTargetWorldPosition.y, SingleFrameZ), 1, 1, PremoveTargetColor);
        }
    }

    public void Push(Move move)
    {
        queue.Enqueue(move);
        boardController.AnimateMove(move, true, true);
    }

    public void ExecuteNext()
    {
        if (!HasMoves()) return;

        var premove = queue.Dequeue();
        boardController.ResetPieces(gameController.gameState);

        gameController.PlayMove(premove);

        foreach (var nextPremove in GetMoves())
        {
            boardController.AnimateMove(nextPremove, true, false);
        }
    }

    public bool HasMoves()
    {
        return queue.Count > 0;
    }

    public void Clear(GameState gameState)
    {
        queue.Clear();
        boardController.ResetPieces(gameState);
        promotionHandler.CancelPromotion();
    }

    private List<Move> GetMoves()
    {
        return queue.ToList();
    }
}
