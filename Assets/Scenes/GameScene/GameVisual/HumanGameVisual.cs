
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class HumanGameVisual : GameVisual
{
    protected PromotionHandler promotionHandler { get; private set; }
    protected HistoryHandler historyHandler { get; private set; }
    protected GameObject selectedPiece { get; private set; }
    protected BoardPosition startPosition { get; private set; }

    public HumanGameVisual()
    {
        promotionHandler = StaticReferences.promotionHandler.Value;
        historyHandler = StaticReferences.historyHandler.Value;
    }

    protected GameObject GetPointerCollision()
    {
        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue()));
        return rayHit.collider != null ? rayHit.collider.gameObject : null;
    }

    protected BoardPosition GetPointerBoardPosition()
    {
        var worldMousePosition = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        return boardController.WorldPositionToBoardPosition(worldMousePosition);
    }

    protected void StartDrawPieceToPointer(GameObject gameObject, BoardPosition startPosition)
    {
        selectedPiece = gameObject;
        this.startPosition = startPosition;
        selectedPiece.GetComponent<SpriteRenderer>().sortingLayerName = "Animation";
    }

    protected void CancelDrawPieceToPointer()
    {
        if (selectedPiece == null) return;
        boardController.AnimatePiece(selectedPiece, startPosition, PieceType.Nothing, true);
        var spriteRenderer = selectedPiece.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Pieces";
        selectedPiece = null;
    }

    protected void ForceStopDrawPieceToPointer()
    {
        if (selectedPiece == null) return;

        var spriteRenderer = selectedPiece.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Pieces";
        selectedPiece = null;
    }

    public override void GameOver(GameStateInterface gameState)
    {
        base.GameOver(gameState);
        if (selectedPiece == null) return;

        var finalSelectedPiecePosition = boardController.GetPieceAtGameObject(selectedPiece);

        if (finalSelectedPiecePosition == null)
        {
            ForceStopDrawPieceToPointer();
        }

        // If the AI ends the game, we want the piece being dragged to return to the game state position, not the start position
        startPosition = finalSelectedPiecePosition.position;
        CancelDrawPieceToPointer();
    }

    public override void HistoryBack()
    {
        base.HistoryBack();
        ForceStopDrawPieceToPointer();
    }

    public override void Update()
    {
        UpdateSelectedHover();
        UpdatePromotionHover();
    }

    protected virtual void UpdateSelectedHover()
    {
        if (!selectedPiece) return;

        // Sometimes the board is reset while a piece is being held, which changes its sorting layer
        selectedPiece.GetComponent<SpriteRenderer>().sortingLayerName = "Animation";

        var mousePosition = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        selectedPiece.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
    }

    private void UpdatePromotionHover()
    {
        if (!promotionHandler.PromotionInProgress) return;

        var collision = GetPointerCollision();

        if (!collision) return;

        promotionHandler.UpdateHoverPromotion(collision);
    }
}
