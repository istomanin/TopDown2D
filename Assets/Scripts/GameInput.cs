using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{

    private PlayerInputActions _playerInputActions;

    public static GameInput Instance { get; private set; }


    public event EventHandler OnPlayerAttack;

    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Combat.Attack.started += PlayerAttack_started;

    }

    public Vector2 GetMovementVector()
    {
        Vector2 _inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return _inputVector;
    }


    public Vector3 GetMousePosition()
    {
        Vector3 _mousePos = Mouse.current.position.ReadValue();
        return _mousePos;
    }

    public void DisableMovment()
    {
        _playerInputActions.Disable();
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack.Invoke(this, EventArgs.Empty);
    }

}
