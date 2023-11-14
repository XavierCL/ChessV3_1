using UnityEngine;

public class PremoveHandler : MonoBehaviour
{
    public float SingleFrameZ = 0.01f;

    public Color PremoveTargetColor = Color.green;
    private BoardController boardController;
    private Shapes shapes;
    private PromotionHandler promotionHandler;

    void Start()
    {
        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        promotionHandler = GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>();
        shapes = GameObject.Find(nameof(Shapes)).GetComponent<Shapes>();
    }

    void Update()
    {
        if (promotionHandler.PromotionInProgress) return;

        foreach (var premove in GameObject.Find(nameof(PremoveQueue)).GetComponent<PremoveQueue>().GetMoves())
        {
            var premoveTargetWorldPosition = boardController.BoardPositionToWorldPosition(premove.target);
            shapes.Rectangle(new Vector3(premoveTargetWorldPosition.x, premoveTargetWorldPosition.y, SingleFrameZ), 1, 1, PremoveTargetColor);
        }
    }
}
