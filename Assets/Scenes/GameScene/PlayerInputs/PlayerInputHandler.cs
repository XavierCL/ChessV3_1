using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedPiece;

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
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
        if (!rayHit.collider) return;
        selectedPiece = rayHit.collider.gameObject;
        var spriteRenderer = selectedPiece.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 2;
    }

    private void OnRelease()
    {
        if (!selectedPiece) return;
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
