using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, PlayerInputAction.IGamePlayActions, PlayerInputAction.IPauseMenuActions
{
    public event UnityAction<Vector2> onMove = delegate { };

    public event UnityAction onStopMove = delegate { };

    public event UnityAction onFire = delegate { };

    public event UnityAction onStopFire = delegate { };

    public event UnityAction onDodge = delegate { };

    public event UnityAction onOverdrive = delegate { };

    public event UnityAction onPause = delegate { };

    public event UnityAction onUnpause = delegate { };

    PlayerInputAction playerInputAction;

    void OnEnable()
    {
        playerInputAction = new PlayerInputAction();

        playerInputAction.GamePlay.SetCallbacks(this);

        playerInputAction.PauseMenu.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    void SwitchActionMap(InputActionMap actionMap, bool isUIInput)
    {
        playerInputAction.Disable();
        actionMap.Enable();

        if (isUIInput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }


    public void SwitchToDynamicUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    public void SwitchToFixedUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;

    public void DisableAllInputs() => playerInputAction.Disable();

    public void EnableGamePlayInput() => SwitchActionMap(playerInputAction.GamePlay, false);

    public void EnablePauseMenuInput() => SwitchActionMap(playerInputAction.PauseMenu, true);

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }

        if(context.canceled)
        {
            onStopMove.Invoke();
        }

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onFire.Invoke();
        }

        if (context.canceled)
        {
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverdrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            onPause.Invoke();
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onUnpause.Invoke();
        }
    }
}
