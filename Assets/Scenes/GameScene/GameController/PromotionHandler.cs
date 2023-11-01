using System.Collections.Generic;
using UnityEngine;

public class PromotionHandler : MonoBehaviour
{
    private GameObject promotionRook;
    private GameObject promotionKnight;
    private GameObject promotionBishop;
    private GameObject promotionQueen;
    private BoardPosition? promotionStartPosition;
    private BoardPosition? promotionEndPosition;
    public bool PromotionInProgress { get => promotionStartPosition != null; }
    public Color CurrentTargetColor = Color.green;

    void Start()
    {
        promotionRook = GameObject.Find("promotionRook");
        promotionKnight = GameObject.Find("promotionKnight");
        promotionBishop = GameObject.Find("promotionBishop");
        promotionQueen = GameObject.Find("promotionQueen");
        gameObject.SetActive(false);
    }

    public void PromptPromotion(BoardPosition source, BoardPosition target)
    {
        var gameController = GameObject.Find(nameof(GameController)).GetComponent<GameController>();
        var boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        var pieceSprites = GameObject.Find(nameof(PieceSprites)).GetComponent<PieceSprites>();

        if (gameController.gameState.whiteTurn)
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

    public void UpdateHoverPromotion(GameObject collider)
    {
        if (!PromotionInProgress) return;
        var promotionGameObjects = new List<GameObject> { promotionRook, promotionKnight, promotionBishop, promotionQueen };
        if (!promotionGameObjects.Contains(collider)) return;
        Shapes.Rectangle(new Vector3(collider.transform.position.x, collider.transform.position.y, 0), 1.1f, 1, CurrentTargetColor);
    }

    public void FinalizePromotion(GameObject collider)
    {
        if (!PromotionInProgress) return;

        var gameController = GameObject.Find(nameof(GameController)).GetComponent<GameController>();
        if (collider == promotionRook)
        {
            gameController.PlayAnimatedMove(new Move(promotionStartPosition.Value, promotionEndPosition.Value, gameController.gameState.whiteTurn ? PieceType.WhiteRook : PieceType.BlackRook), false);
        }
        else if (collider == promotionKnight)
        {
            gameController.PlayAnimatedMove(new Move(promotionStartPosition.Value, promotionEndPosition.Value, gameController.gameState.whiteTurn ? PieceType.WhiteKnight : PieceType.BlackKnight), false);
        }
        else if (collider == promotionBishop)
        {
            gameController.PlayAnimatedMove(new Move(promotionStartPosition.Value, promotionEndPosition.Value, gameController.gameState.whiteTurn ? PieceType.WhiteBishop : PieceType.BlackBishop), false);
        }
        else if (collider == promotionQueen)
        {
            gameController.PlayAnimatedMove(new Move(promotionStartPosition.Value, promotionEndPosition.Value, gameController.gameState.whiteTurn ? PieceType.WhiteQueen : PieceType.BlackQueen), false);
        }
        else
        {
            // Unrelated game object was collided with, return
            return;
        }

        promotionStartPosition = null;
        promotionEndPosition = null;
        gameObject.SetActive(false);
    }
}
