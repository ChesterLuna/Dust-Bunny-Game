using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    #region  Variables
    public PlayerStates PlayerState = PlayerStates.Playing;

    #region Camera
    // Camera
    [Header("Camera")]
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;
    #endregion

    #region Movement
    // Movement variables
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 500f;
    // [SerializeField] float _moveSmoothing = 0.05f;
    [SerializeField] float _accelerationForce = 1f;
    [SerializeField] float _deaccelerationForce = 1f;
    [SerializeField] float _groundFriction = 10f;
    [SerializeField] float _airFriction = 1f;
    #endregion

    #region Abilities
    // Abilities variables
    [Header("Abilities")]
    [SerializeField] float _jumpForce = 22f;
    [SerializeField] float _coyoteTime = 0.1f;
    [SerializeField] float _dashForce = 22f;
    [SerializeField] float _dashTime = 0.2f;
    [SerializeField] float _dashCooldown = 0.1f;
    float _lastTimeGrounded = 0f;
    float _lastTimeDashed = 0f;
    #endregion

    #region Size Changing
    // Size Changing variables
    [Header("Size Changing")]
    [Range(0, 5)][SerializeField] int _bunnySize = 1;
    [SerializeField] float _bunnySizeScalar = 1.5f;
    [SerializeField] float _scaleSpeed = 1.6f;
    [SerializeField] float _moveSpeedAddition = 1f;
    [SerializeField] float _jumpForceAddition = 2f;
    [SerializeField] float _dashForceAddition = 2f;
    Coroutine _lastDamp;

    float _epsilon = 0.01f;
    Vector3 _originalSize = new Vector3(1f, 1f, 1f);
    float _originalMoveSpeed = 10f;
    float _originalJumpForce = 22f;
    float _originalDashForce = 12f;
    #endregion

    #region Dust
    // Dust Changing variables
    [Header("Dust Values")]
    [SerializeField] float _dust = 100f;
    [SerializeField] float _maxDust = 100f;
    [SerializeField] float[] _dustLevels;
    #endregion

    #region Abilities
    // Abilities booleans
    bool _canJump = true;
    bool _canDash = true;
    bool _isDashing = false;
    bool _growing = false;
    bool _grounded = false;
    bool _feetGrounded = false;
    bool _isCoyoteTime = false;
    float _stickyHeightDivisor = 1f;
    bool _doJump = false;
    bool _doDash = false;
    #endregion

    #region Platforms
    // Platform variables
    private Transform _originalParent = null;
    private DropDownPlatform _currentDropDownPlatform = null;
    private RigidBodyRideable _oldRidable = null;
    private Transform _oldMovingPlatform = null;
    private Collider2D _lastCollision = null;
    #endregion

    #region Physics
    // Physics
    [Header("Physics")]
    Rigidbody2D _thisRigidbody;
    float _standardGravity;
    Collider2D _thisCollider;
    Vector2 _previousVelocity;
    [SerializeField] LayerMask _environmentLayer;
    int _layerMaskValue;
    #endregion

    #region Input
    // Input
    Vector2 _dashDirection = Vector2.zero;
    Vector2 _jumpForceVector = Vector2.zero;
    public bool IsFacingRight = true;
    #endregion

    #region SFX
    // SFX
    PlayerSFXController _sfx;
    // Death
    [Header("Death")]
    [SerializeField] private Animator _deathTransition;
    [SerializeField] private float _deathTransitionTime = 1f;
    #endregion

    #region External References
    public bool IsDashing => _isDashing;
    public bool CanJump => _canJump;
    public bool IsGrounded => _grounded;
    public bool IsCoyoteTime => _isCoyoteTime;
    public bool IsGrowing => _growing;
    public bool IsMaxedOutDust => _dust >= _maxDust;
    public bool IsOnDropDownPlatform => _currentDropDownPlatform != null;
    public bool IsOnRideable => _oldRidable != null;
    public bool IsOnMovingPlatform => _oldMovingPlatform != null;
    public float Dust
    {
        get => _dust;
        set => _dust = value;
    }
    public float MaxDust => _maxDust;
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }
    public float JumpForce
    {
        get => _jumpForce;
        set => _jumpForce = value;
    }
    public float DashForce
    {
        get => _dashForce;
        set => _dashForce = value;
    }

    public int BunnySize => _bunnySize;
    public Rigidbody2D RB => _thisRigidbody;
    public Collider2D Col => _thisCollider;
    public PlayerSFXController SFX => _sfx;
    public float StickyHeightDivisor
    {
        get => _stickyHeightDivisor;
        set => _stickyHeightDivisor = value;
    }
    #endregion

    #region Actions
    public event Action Dead;
    public event Action Jumped;
    public event Action DoubleJumped;
    public event Action<bool, float> GroundedChanged; // Grounded - Impact force
    public event Action<bool, Vector2> DashingChanged; // Dashing - Dir
    #endregion
    #endregion

    private void Awake()
    {
        _thisRigidbody = GetComponent<Rigidbody2D>();
        _standardGravity = _thisRigidbody.gravityScale;
        _thisCollider = GetComponent<Collider2D>();
        _cameraFollowObject = FindObjectOfType<CameraFollowObject>();
        _originalSize = transform.localScale;
        _originalMoveSpeed = _moveSpeed;
        _originalJumpForce = _jumpForce;
        _originalDashForce = _dashForce;
        _layerMaskValue = Mathf.RoundToInt(Mathf.Log(_environmentLayer.value, 2));

        _sfx = gameObject.GetComponentInChildren<PlayerSFXController>();
    } // end Awake

    private void Start()
    {
        GameManager.instance.StartGameTime();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
        if (GameManager.instance.CheckpointLocation != Vector3.zero)
        {
            transform.position = GameManager.instance.CheckpointLocation;
        }
    } // end Start

    void Update()
    {
        gatherInput();
        if (PlayerState != PlayerStates.Playing) return;
        //updateFriction(); //TODO: Ensure it doesnt flipflop
        updateDustSize();
        updateTimers();
        updateCameraYDamping();

    } // end Update

    void updateTimers()
    {
        _lastTimeGrounded += Time.deltaTime;

        if (!_isDashing)
            _lastTimeDashed += Time.deltaTime;
    } // end updateTimers

    void FixedUpdate()
    {
        TurnCheck();
        checkGrounded();
        if (PlayerState == PlayerStates.Dialogue)
        {
            _doJump = false;
            _doDash = false;
            _thisRigidbody.velocity = new Vector2(0, _thisRigidbody.velocity.y);
            return;
        }

        if (!_isDashing)
        {
            Move();
        }

        // _thisRigidbody.velocity = Vector2.SmoothDamp(_thisRigidbody.velocity, targetVelocity, ref _zeroVelocity, _moveSmoothing);
        if (_doJump && _canJump && _grounded)
        {
            Jump();
        }
        Debug.Log(_doDash + " " + _canDash + " " + (_lastTimeDashed > _dashCooldown));
        if (_doDash && _canDash && _lastTimeDashed > _dashCooldown)
        {
            StartCoroutine(Dash());
        }
        _doJump = false;
        _doDash = false;
        _previousVelocity = _thisRigidbody.velocity;
    } // end FixedUpdate

    private void gatherInput()
    {
        if (!(PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Dialogue)) return;

        if (UserInput.instance.InteractInput)
        {
            sendInteract();
        }

        if (PlayerState != PlayerStates.Playing) return;

        if (UserInput.instance.JumpJustPressed)
        {
            _doJump = true;
        }

        if (UserInput.instance.DashInput)
        {
            _doDash = true;
        }
    } // end gatherInput

    #region Animations
    private void TurnCheck()
    {
        if (UserInput.instance.MoveInput.x > 0 && !IsFacingRight)
        {
            Turn();
        }
        else if (UserInput.instance.MoveInput.x < 0 && IsFacingRight)
        {
            Turn();
        }
    } // end TurnCheck

    public void Turn()
    {
        Vector3 rotator;
        if (IsFacingRight)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        }
        transform.rotation = Quaternion.Euler(rotator);
        IsFacingRight = !IsFacingRight;
        _cameraFollowObject.CallTurn();
    } // end Turn
    #endregion

    #region Cameras
    private void updateCameraYDamping()
    {
        // If we are falling past a certain speed threshold
        if (_thisRigidbody.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        if (_thisRigidbody.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    } // end updateCameraYDamping
    #endregion

    private void checkGrounded()
    {
        // Raycast to check if the player is grounded
        // RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f * transform.localScale.y, _environmentLayer);
        // Debug.DrawRay(transform.position, Vector2.down, Color.red); // Draw the raycast

        // _grounded = (hit.collider != null) && !(hit.collider == null && _lastTimeGrounded < _coyoteTime);

        _grounded = _feetGrounded && !(!_feetGrounded && _lastTimeGrounded < _coyoteTime);

        if (_grounded)
        {
            GroundedChanged?.Invoke(true, Mathf.Abs(_previousVelocity.y));
            _lastTimeGrounded = 0f;
            _canJump = true;
            _canDash = true;
        }
        else
        {
            GroundedChanged?.Invoke(false, 0);
        }


        _currentDropDownPlatform = _lastCollision?.GetComponentInChildren<DropDownPlatform>();
        // Handle Ridable Calculations
        RigidBodyRideable currentRidable = _lastCollision?.GetComponentInParent<RigidBodyRideable>();
        if (currentRidable != _oldRidable)
        {
            _oldRidable?.RemoveRider(this);
            currentRidable?.SetRider(this);
        }
        _oldRidable = currentRidable;
    } // end checkGrounded

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _layerMaskValue)
        {
            _feetGrounded = true;
        }

        _lastCollision = other;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == _layerMaskValue)
        {
            _feetGrounded = false;
        }

        _lastCollision = null;
    }


    void Move()
    {
        float _targetSpeed = UserInput.instance.MoveInput.x * _moveSpeed;
        float _speedToTargetSpeed = _targetSpeed - _thisRigidbody.velocity.x;
        float _accelerationRate;

        if (MathF.Abs(_targetSpeed) > 0.01f)
        {
            _accelerationRate = _accelerationForce;
        }
        else
        {
            _accelerationRate = _deaccelerationForce;
        }

        float _horizontalMovement = Mathf.Abs(_speedToTargetSpeed) * _accelerationRate;

        Vector2 _newMovement = new Vector2(MathF.Sign(_speedToTargetSpeed) * Mathf.Pow(_horizontalMovement, 0.9f), 0);// * Time.fixedDeltaTime, 0);
        _thisRigidbody.AddForce(_newMovement, ForceMode2D.Force);
    } // end Move


    void updateFriction()
    {
        float _friction;

        if (_grounded)
        {
            _friction = _groundFriction;
        }
        else
        {
            _friction = _airFriction;
        }

        if (Mathf.Abs(UserInput.instance.MoveInput.x) == 0f)
        {
            Vector2 _addFrictionVector = Mathf.Sign(_thisRigidbody.velocity.x) * new Vector2(_friction * Time.deltaTime, 0);

            _thisRigidbody.AddForce(_addFrictionVector, ForceMode2D.Impulse);
        }
    } // end updateFriction

    void Jump()
    {
        if (_currentDropDownPlatform != null && UserInput.instance.MoveInput.y < 0f)
        {
            _currentDropDownPlatform.DropDown(_thisCollider);
        }
        else
        {
            //_thisRigidbody.velocity = new Vector2(_thisRigidbody.velocity.x, 0f);
            _thisRigidbody.velocity = Vector2.right * _thisRigidbody.velocity;
            _jumpForceVector = new Vector2(0f, _jumpForce / _stickyHeightDivisor);
            _thisRigidbody.AddForce(_jumpForceVector, ForceMode2D.Impulse);
        }
        _canJump = false;
        Jumped?.Invoke();
    } // end Jump

    // Starts a timer for _coyoteTime seconds, once finished it will set grounded to false
    IEnumerator startCoyoteTime()
    {
        _isCoyoteTime = true;
        _grounded = true;
        yield return new WaitForSeconds(_coyoteTime);
        _isCoyoteTime = false;
        _grounded = false;
    } // end startCoyoteTime


    // Dashes for "_dashTime" seconds constantly. Uses AddForce.
    IEnumerator Dash()
    {
        Debug.Log("Dashing");
        _isDashing = true;
        _lastTimeDashed = 0f;

        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        _dashDirection = (UserInput.instance.DashPositionInput - playerPos).normalized;

        _thisRigidbody.gravityScale = 0f;
        _thisRigidbody.velocity = Vector2.zero;

        _thisRigidbody.velocity = new Vector2(_dashDirection.x * _dashForce, _dashDirection.y * _dashForce);
        DashingChanged?.Invoke(true, _dashDirection);
        yield return new WaitForSeconds(_dashTime);
        _thisRigidbody.gravityScale = _standardGravity;
        _thisRigidbody.velocity = Vector2.zero;

        _lastTimeDashed = 0f;
        _canDash = false;
        _isDashing = false;
        DashingChanged?.Invoke(false, Vector2.zero);
    } // end Dash

    private void sendInteract()
    {
        ContactFilter2D cf = new ContactFilter2D().NoFilter();
        List<Collider2D> results = new List<Collider2D>();

        _thisCollider.OverlapCollider(cf, results);
        foreach (Collider2D collider in results)
        {
            IInteractable interactScript = collider.gameObject.GetComponent<IInteractable>();
            if (interactScript != null)
            {
                interactScript.Interact();
            }
        }
    } // end sendInteract

    #region Dust Functions
    void updateDustSize()
    {
        int _newDustSize = _bunnySize;
        for (int i = 0; i < _dustLevels.Length; i++)
        {
            if (_dust >= _dustLevels[i])
            {
                _newDustSize = i;
            }
        }
        if (_newDustSize != _bunnySize)
        {
            ChangeSize(_newDustSize);
        }
    } // end updateDustSize

    public void ChangeSize(int newSize)
    {
        if (_growing == true || PlayerState == PlayerStates.Dead)
            return;
        Vector3 targetScale = _originalSize * Mathf.Pow(_bunnySizeScalar, newSize - 1);
        _bunnySize = newSize;

        _moveSpeed = _originalMoveSpeed - _moveSpeedAddition * (newSize - 1);
        _jumpForce = _originalJumpForce + _jumpForceAddition * (newSize - 1);
        _dashForce = _originalDashForce + _dashForceAddition * (newSize - 1);
        if (_lastDamp != null)
            StopCoroutine(_lastDamp);
        _lastDamp = StartCoroutine(GrowDamp(targetScale));
    }

    IEnumerator GrowDamp(Vector3 targetScale)
    {
        _growing = true;
        while (Vector3.Distance(transform.localScale, targetScale) > _epsilon)
        {
            Vector3 interValue = Vector3.Lerp(transform.localScale, targetScale, _scaleSpeed * Time.deltaTime);
            transform.localScale = interValue;
            yield return null;
        }
        _growing = false;
    }
    #endregion

    // Deprecated. To Make player a parent of the platform.
    private void parentToPlatform(RaycastHit2D hit)
    {
        Transform currentMovingPlatform = hit.collider.GetComponentInParent<MovingPlatform>()?.transform;

        // if (currentMovingPlatform != null)
        // {
        //     if (currentMovingPlatform != _oldMovingPlatform)
        //     {
        //         if (_oldMovingPlatform != null)
        //         {
        //             resetParent();
        //         }
        //         SetParent(currentMovingPlatform);
        //     }
        // }
        // else
        // {
        //     resetParent();
        // }
        // _oldMovingPlatform = currentMovingPlatform;

    }
    #region Helper Functions
    public void SetParent(Transform newParent)
    {
        //_originalParent = transform.parent;
        transform.parent = newParent;
    } // end SetParent

    public void ResetParent()
    {
        transform.parent = _originalParent;
    } // end ResetParent


    public void AddDust(float scalar)
    {
        _dust += scalar;
        if (_dust > _maxDust)
        {
            _dust = _maxDust;
        }
    } // end AddDust

    public void RemoveDust(float scalar)
    {
        if (_dust - scalar <= 0 && PlayerState != PlayerStates.Dead)
        {
            Die();
            return;
        }
        _dust -= scalar;
    } // end RemoveDust
    void setParent(Transform newParent)
    {
        _originalParent = transform.parent;
        transform.parent = newParent;
    } // end setParent

    void resetParent()
    {
        transform.parent = _originalParent;
    } // end resetParent
    #endregion

    public void Die()
    {
        Dead?.Invoke();
        PlayerState = PlayerStates.Dead;
        _thisCollider.enabled = false;
        _thisRigidbody.simulated = false;
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevel(SceneManager.GetActiveScene().name, _deathTransition, _deathTransitionTime);
        GameManager.instance.PauseGameTime();
    } // end Die


    public enum PlayerStates
    {
        Playing,
        Paused,
        Dialogue,
        Dead
    }
} // end class PlayerController
