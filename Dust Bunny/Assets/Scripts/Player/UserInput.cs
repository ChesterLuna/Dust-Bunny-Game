using UnityEngine;

using UnityEngine.InputSystem;


public class UserInput : MonoBehaviour
{
    public static UserInput instance;
    public bool UseMouseForDash = false;
    // private PlayerInputActions _actions;
    private InputActionAsset _actions;

    private InputAction _move, _jump, _dash, _dashPosition, _interact, _menu;
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
        _actions = GetComponent<PlayerInput>().actions;

        SetUpInputActions();
    } // end Awake

    private void SetUpInputActions()
    {
        _move = _actions["Move"];
        _dashPosition = _actions["DashPosition"];
        _jump = _actions["Jump"];
        _dash = _actions["Dash"];
        _interact = _actions["Interact"];
        _menu = _actions["ToggleMenu"];
    } // end SetUpInputActions


    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    public FrameInput Gather()
    {
        return new FrameInput
        {
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            DashDown = _dash.WasPressedThisFrame(),
            Move = _move.ReadValue<Vector2>(),
            DashDirection = _dashPosition.ReadValue<Vector2>(),
            InteractDown = _interact.WasPressedThisFrame(),
            MenuDown = _menu.WasPressedThisFrame()
        };
    } // end Gather

} // end class PlayerInput

public struct FrameInput
{
    public Vector2 Move;
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public Vector2 DashDirection;
    public bool InteractDown;
    public bool MenuDown;
} // end struct FrameInput
