using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private GameController gameController;

    void Awake()
    {
        gameController = StaticReferences.gameController.Value;
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
}
