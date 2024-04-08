using UnityEngine;

using UnityEngine.InputSystem;


public class UserInput : MonoBehaviour
{
    public static UserInput instance;
    public bool UseMouseForDash { get; private set; }
    // private PlayerInputActions _actions;
    private InputActionAsset _actions;

    private InputAction _move, _jump, _dash, _dashPosition, _interact, _menu, _anyKey;
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

    void FixedUpdate(){
        // Refresh the input system
        InputSystem.Update();
    }

    void Update(){
        // Refresh the input system manually, only if TimeScale = 0
        if (Time.timeScale == 0) InputSystem.Update();
    }

    public void SetMouseForDash(bool value)
    {
        UseMouseForDash = value;
        PlayerPrefs.SetInt("UseMouseForDash", value ? 1 : 0);
    } // end SetMouseForDash

    private void SetUpInputActions()
    {
        UseMouseForDash = PlayerPrefs.GetInt("UseMouseForDash", 1) == 1;
        _move = _actions["Move"];
        _dashPosition = _actions["DashPosition"];
        _jump = _actions["Jump"];
        _dash = _actions["Dash"];
        _interact = _actions["Interact"];
        _menu = _actions["ToggleMenu"];
        _anyKey = _actions["AnyKey"];
    } // end SetUpInputActions


    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    public FrameInput Gather(PlayerStates playerState = PlayerStates.Playing)
    {
        // Check states
        if (playerState == PlayerStates.Paused)
        {
            return new FrameInput
            {
                JumpDown = false,
                JumpHeld = false,
                DashDown = false,
                Move = Vector2.zero,
                DashDirection = Vector2.zero,
                InteractDown = false,
                MenuDown = _menu.WasReleasedThisFrame(),
                AnyKey = _anyKey.WasPressedThisFrame()
            };
        }
        else if (playerState == PlayerStates.Dialogue)
        {
            return new FrameInput
            {
                JumpDown = false,
                JumpHeld = false,
                DashDown = false,
                Move = Vector2.zero,
                DashDirection = Vector2.zero,
                InteractDown = _interact.WasPressedThisFrame(),
                MenuDown = _menu.WasPressedThisFrame(),
                AnyKey = _anyKey.WasPressedThisFrame() && !_interact.WasPerformedThisFrame()
            };
        }
        else
        {
            return new FrameInput
            {
                JumpDown = _jump.WasPressedThisFrame(),
                JumpHeld = _jump.IsPressed(),
                DashDown = _dash.WasPressedThisFrame(),
                Move = _move.ReadValue<Vector2>(),
                DashDirection = _dashPosition.ReadValue<Vector2>(),
                InteractDown = _interact.WasPressedThisFrame(),
                MenuDown = _menu.WasPressedThisFrame(),
                AnyKey = _anyKey.WasPressedThisFrame()
            };
        }
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
    public bool AnyKey;
} // end struct FrameInput
