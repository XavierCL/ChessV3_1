using UnityEngine;

public class PremoveHandler : MonoBehaviour
{
    public float SingleFrameZ = 0.01f;

    public Color PremoveTargetColor = Color.green;
    private BoardController boardController;
    private Shapes shapes;
    private PromotionHandler promotionHandler;
    private PremoveQueue premoveQueue;

    void Awake()
    {
        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        promotionHandler = GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>();
        premoveQueue = GameObject.Find(nameof(PremoveQueue)).GetComponent<PremoveQueue>();
        shapes = GameObject.Find(nameof(Shapes)).GetComponent<Shapes>();

        // todo move premove queue here, and on empty also reset the board position, on pop reset the board position and recurse following, etc.
    }

    void Update()
    {
        if (promotionHandler.PromotionInProgress) return;

        foreach (var premove in premoveQueue.GetMoves())
        {
            var premoveTargetWorldPosition = boardController.BoardPositionToWorldPosition(premove.target);
            shapes.Rectangle(new Vector3(premoveTargetWorldPosition.x, premoveTargetWorldPosition.y, SingleFrameZ), 1, 1, PremoveTargetColor);
        }
    }
}
