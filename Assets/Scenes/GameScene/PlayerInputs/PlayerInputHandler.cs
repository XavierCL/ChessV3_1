using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private GameController gameController;
    private HistoryHandler historyHandler;
    private AiController aiController;

    void Awake()
    {
        gameController = StaticReferences.gameController.Value;
        historyHandler = StaticReferences.historyHandler.Value;
        aiController = StaticReferences.aiController.Value;
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
        if (context.phase == InputActionPhase.Started)
        {
            historyHandler.Backward();
        }
    }

    public void OnNavigateForward(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            historyHandler.Forward();
        }
    }

    public async void OnAiRethink(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            var historicGame = historyHandler.GetGameAtHistory();
            aiController.ResetAis();
            await aiController.GetMove(historicGame, gameController.IsAi1Turn(historicGame), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }
}
