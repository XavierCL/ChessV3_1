using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedPiece;
    private BoardPosition startPosition;

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
        // Check human turn
        var gameController = GameObject.Find(nameof(GameController)).GetComponent<GameController>();
        if (gameController.gameType == GameType.Ai1WhiteAi2Black || gameController.gameType == GameType.Ai1BlackAi2White) return;
        if (gameController.gameType != GameType.HumanHuman)
        {
            if (gameController.gameType == GameType.HumanWhiteAiBlack && !gameController.gameState.whiteTurn) return;
            if (gameController.gameType == GameType.HumanBlackAiWhite && gameController.gameState.whiteTurn) return;
        }

        // Check collision
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
        if (!rayHit.collider) return;

        // Check legal moves
        var boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        var worldMousePosition = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        var startBoardPosition = boardController.WorldPositionToBoardPosition(worldMousePosition);
        if (!gameController.gameState.getLegalMoves().Any(move => move.source.Equals(startBoardPosition))) return;

        // Set state
        selectedPiece = rayHit.collider.gameObject;
        startPosition = startBoardPosition;
        selectedPiece.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private void OnRelease()
    {
        if (!selectedPiece) return;

        // Check legal moves
        var gameController = GameObject.Find(nameof(GameController)).GetComponent<GameController>();
        var boardController = GameObject.Find(nameof(BoardController)).GetComponent<BoardController>();
        var endBoardPosition = boardController.WorldPositionToBoardPosition(mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue()));
        var moveAttempt = new Move(startPosition, endBoardPosition, null);
        if (!gameController.gameState.getLegalMoves().Any(move => move.Equals(moveAttempt)))
        {
            // Reset position
            var originalPosition = boardController.BoardPositionToWorldPosition(startPosition);
            boardController.AnimatePiece(selectedPiece, startPosition, null);
        }
        else
        {
            // Play move
            var finalPosition = boardController.BoardPositionToWorldPosition(endBoardPosition);
            selectedPiece.transform.position = new Vector3(finalPosition.x, finalPosition.y, 0);
            gameController.PlayImmediateMove(moveAttempt);
        }

        var spriteRenderer = selectedPiece.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        selectedPiece = null;
    }

    public void Update()
    {
        if (!selectedPiece) return;
        var mousePosition = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        selectedPiece.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
    }
}
