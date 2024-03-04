using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{

    public static UserInput instance;
    public Vector2 MoveInput { get; private set; }
    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool JumpJustReleased { get; private set; }
    public bool DashInput { get; private set; }
    public Vector2 DashPositionInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool ToggleMenu { get; private set; }

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _dashPositionAction;
    private InputAction _interactAction;
    private InputAction _menuAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _playerInput = GetComponent<PlayerInput>();

        SetUpInputActions();
    } // end Awake

    private void Update()
    {
        UpdateInputs();
    } // end Update

    private void SetUpInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _dashPositionAction = _playerInput.actions["DashPosition"];
        _jumpAction = _playerInput.actions["Jump"];
        _dashAction = _playerInput.actions["Dash"];
        _interactAction = _playerInput.actions["Interact"];
        _menuAction = _playerInput.actions["ToggleMenu"];
    } // end SetUpInputActions

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        JumpJustReleased = _jumpAction.WasReleasedThisFrame();
        DashInput = _dashAction.WasPressedThisFrame();
        DashPositionInput = _dashPositionAction.ReadValue<Vector2>();
        InteractInput = _interactAction.WasPressedThisFrame();
        ToggleMenu = _menuAction.WasPressedThisFrame();
        Debug.Log("ToggleMenu: " + ToggleMenu);
    } // end UpdateInputs
} // end class UserInput
