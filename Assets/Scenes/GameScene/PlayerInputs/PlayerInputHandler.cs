using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private GameController gameController;
    private HistoryHandler historyHandler;

    void Awake()
    {
        gameController = StaticReferences.gameController.Value;
        historyHandler = StaticReferences.historyHandler.Value;
    }

    public void OnPressPointerEvent(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            gameController.gameVisual.BoardMousePress();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            gameController.gameVisual.BoardMouseRelease();
        }
    }

    public void OnNavigateBack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            historyHandler.Backward();
        }
    }

    public void OnNavigateForward(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            historyHandler.Forward();
        }
    }
}
