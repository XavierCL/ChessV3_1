using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedPiece;
    private BoardPosition startPosition;
    private BoardController boardController;
    private GameController gameController;
    private PromotionHandler promotionHandler;

    void Awake()
    {
        promotionHandler = GameObject.Find(nameof(PromotionHandler)).GetComponent<PromotionHandler>();
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPressPointerEvent(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnPress();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnRelease();
        }
    }

    private void OnPress()
    {
        // Check promotion in progress
        if (promotionHandler.PromotionInProgress)
        {
            HandlePromotionPress();
            return;
        }

        // Check human turn
        var gameController = GetGameController();
        if (gameController.gameType == GameType.Ai1WhiteAi2Black || gameController.gameType == GameType.Ai1BlackAi2White) return;
        if (gameController.gameType != GameType.HumanHuman)
        {
            if (gameController.gameType == GameType.HumanWhiteAiBlack && !gameController.gameState.whiteTurn) return;
            if (gameController.gameType == GameType.HumanBlackAiWhite && gameController.gameState.whiteTurn) return;
        }

        // Check collision
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
        if (!rayHit.collider) return;

        // Check own pieces
        var boardController = GetBoardController();
        var worldMousePosition = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        var startBoardPosition = boardController.WorldPositionToBoardPosition(worldMousePosition);
        var pieceAtPosition = gameController.gameState.getPieceAtPosition(startBoardPosition);
        if (gameController.gameState.whiteTurn && pieceAtPosition != PieceType.WhitePawn && pieceAtPosition != PieceType.WhiteRook && pieceAtPosition != PieceType.WhiteKnight && pieceAtPosition != PieceType.WhiteBishop && pieceAtPosition != PieceType.WhiteQueen && pieceAtPosition != PieceType.WhiteKing) return;
        if (!gameController.gameState.whiteTurn && pieceAtPosition != PieceType.BlackPawn && pieceAtPosition != PieceType.BlackRook && pieceAtPosition != PieceType.BlackKnight && pieceAtPosition != PieceType.BlackBishop && pieceAtPosition != PieceType.BlackQueen && pieceAtPosition != PieceType.BlackKing) return;

        // Set state
        selectedPiece = rayHit.collider.gameObject;
        startPosition = startBoardPosition;
        selectedPiece.GetComponent<SpriteRenderer>().sortingLayerName = "Animation";
    }

    private void OnRelease()
    {
        if (!selectedPiece) return;

        // Check legal moves
        var gameController = GetGameController();
        var boardController = GetBoardController();
        var endBoardPosition = boardController.WorldPositionToBoardPosition(mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue()));
        var validMoves = gameController.gameState.getLegalMoves().Where(move => move.source.Equals(startPosition) && move.target.Equals(endBoardPosition)).ToList();
        if (validMoves.Count == 0)
        {
            // Reset position
            var originalPosition = boardController.BoardPositionToWorldPosition(startPosition);
            boardController.AnimatePiece(selectedPiece, startPosition, null, true);
        }
        else
        {
            if (validMoves.Count > 1)
            {
                // Pawn promotion
                promotionHandler.PromptPromotion(startPosition, endBoardPosition);
            }
            else
            {
                // Play move
                var finalPosition = boardController.BoardPositionToWorldPosition(endBoardPosition);
                gameController.PlayAnimatedMove(new Move(startPosition, endBoardPosition, null), false);
            }
        }

        var spriteRenderer = selectedPiece.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Pieces";
        selectedPiece = null;
    }

    public void Update()
    {
        UpdateSelectedHover();
        UpdatePromotionHover();
    }

    private void HandlePromotionPress()
    {
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
        if (!rayHit.collider) return;

        promotionHandler.FinalizePromotion(rayHit.collider.gameObject);
    }

    private void UpdateSelectedHover()
    {
        if (!selectedPiece) return;

        var mousePosition = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        selectedPiece.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        GetBoardController().DrawPossibleTargets(GetGameController().gameState, startPosition);
        GetBoardController().DrawCurrentTarget(GetGameController().gameState, startPosition, new Vector2(mousePosition.x, mousePosition.y));
    }

    private void UpdatePromotionHover()
    {
        if (!promotionHandler.PromotionInProgress) return;

        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
        if (!rayHit.collider) return;
        promotionHandler.UpdateHoverPromotion(rayHit.collider.gameObject);
    }

    private BoardController GetBoardController()
    {
        if (boardController != null) return boardController;

        boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();

        return boardController;
    }

    private GameController GetGameController()
    {
        if (gameController != null) return gameController;

        gameController = GameObject.Find(nameof(GameController)).GetComponent<GameController>();

        return gameController;
    }
}
