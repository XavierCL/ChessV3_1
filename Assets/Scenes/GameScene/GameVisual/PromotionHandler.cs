using System.Collections.Generic;
using UnityEngine;

public class PromotionHandler : MonoBehaviour
{
    private GameObject promotionRook;
    private GameObject promotionKnight;
    private GameObject promotionBishop;
    private GameObject promotionQueen;
    private Shapes shapes;
    private BoardController boardController;
    private GameController gameController;
    private PieceSprites pieceSprites;

    private BoardPosition? promotionStartPosition;
    private BoardPosition? promotionEndPosition;
    public bool PromotionInProgress { get => promotionStartPosition != null; }
    public Color CurrentTargetColor = Color.green;

    void Awake()
    {
        promotionRook = GameObject.Find("promotionRook");
        promotionKnight = GameObject.Find("promotionKnight");
        promotionBishop = GameObject.Find("promotionBishop");
        promotionQueen = GameObject.Find("promotionQueen");
        shapes = StaticReferences.shapes.Value;
        boardController = StaticReferences.boardController.Value;
        gameController = StaticReferences.gameController.Value;
        pieceSprites = StaticReferences.pieceSprites.Value;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void PromptPromotion(BoardPosition source, BoardPosition target, bool isWhite)
    {
        if (isWhite)
        {
            promotionRook.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.WhiteRook);
            promotionKnight.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.WhiteKnight);
            promotionBishop.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.WhiteBishop);
            promotionQueen.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.WhiteQueen);
        }
        else
        {
            promotionRook.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.BlackRook);
            promotionKnight.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.BlackKnight);
            promotionBishop.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.BlackBishop);
            promotionQueen.GetComponent<SpriteRenderer>().sprite = pieceSprites.GetSpriteFor(PieceType.BlackQueen);
        }

        promotionStartPosition = source;
        promotionEndPosition = target;
        var targetWorldPosition = boardController.BoardPositionToWorldPosition(target);
        gameObject.transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y - 1.6f, 0);
        gameObject.SetActive(true);
    }

    public void CancelPromotion()
    {
        promotionStartPosition = null;
        promotionEndPosition = null;
        gameObject.SetActive(false);
    }

    public void UpdateHoverPromotion(GameObject collider)
    {
        if (!PromotionInProgress) return;
        var promotionGameObjects = new List<GameObject> { promotionRook, promotionKnight, promotionBishop, promotionQueen };
        if (!promotionGameObjects.Contains(collider)) return;
        shapes.Rectangle(new Vector3(collider.transform.position.x, collider.transform.position.y, 0), 1.1f, 1, CurrentTargetColor);
    }

    public Move FinalizePromotion(GameObject collider, bool isWhite)
    {
        if (!PromotionInProgress) return null;

        Move moveResult = null;
        if (collider == promotionRook)
        {
            moveResult = new Move(promotionStartPosition.Value, promotionEndPosition.Value, isWhite ? PieceType.WhiteRook : PieceType.BlackRook);
        }
        if (collider == promotionKnight)
        {
            moveResult = new Move(promotionStartPosition.Value, promotionEndPosition.Value, isWhite ? PieceType.WhiteKnight : PieceType.BlackKnight);
        }
        if (collider == promotionBishop)
        {
            moveResult = new Move(promotionStartPosition.Value, promotionEndPosition.Value, isWhite ? PieceType.WhiteBishop : PieceType.BlackBishop);
        }
        if (collider == promotionQueen)
        {
            moveResult = new Move(promotionStartPosition.Value, promotionEndPosition.Value, isWhite ? PieceType.WhiteQueen : PieceType.BlackQueen);
        }

        if (moveResult != null)
        {
            CancelPromotion();
        }

        return moveResult;
    }
}
