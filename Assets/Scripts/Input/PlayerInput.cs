using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, PlayerInputAction.IGamePlayActions
{
    public event UnityAction<Vector2> onMove = delegate { };

    public event UnityAction onStopMove = delegate { };

    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };

    PlayerInputAction playerInputAction;

    void OnEnable()
    {
        playerInputAction = new PlayerInputAction();

        playerInputAction.GamePlay.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInputs();
    }


    public void DisableAllInputs()
    {
        playerInputAction.GamePlay.Disable();
    }

    public void EnableGamePlayInput()
    {
        playerInputAction.GamePlay.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }

        if(context.phase == InputActionPhase.Canceled)
        {
            onStopMove.Invoke();
        }

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onFire.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            onStopFire.Invoke();
        }
    }
}
