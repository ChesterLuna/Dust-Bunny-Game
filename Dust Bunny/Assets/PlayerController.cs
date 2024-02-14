using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Camera
    [Header("Camera")]
    [SerializeField] Camera _mainCamera;

    // Movement variables
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 500f;
    [SerializeField] float _moveSmoothing = 0.05f;
    [SerializeField] float _accelerationForce = 1f;
    [SerializeField] float _deaccelerationForce = 1f;
    [SerializeField] float _groundFriction = 10f;
    [SerializeField] float _airFriction = 1f;


    // Abilities variables
    [Header("Abilities")]
    [SerializeField] float _jumpForce = 22f;
    [SerializeField] float _coyoteTime = 0.1f;
    [SerializeField] float _dashForce = 22f;
    [SerializeField] float _dashTime = 0.2f;
    [SerializeField] float _dashCooldown = 0.1f;
    float _lastTimeGrounded = 0f;
    float _lastTimeDashed = 0f;

    // Size Changing variables
    [Header("Size Changing")]
    [Range(0, 5)] [SerializeField] int _bunnySize = 1;
    [SerializeField] float _bunnySizeScalar = 1.5f;
    [SerializeField] float _scaleSpeed = 1.6f;
    [SerializeField] float _moveSpeedAddition = 1f;
    [SerializeField] float _jumpForceAddition = 2f;
    [SerializeField] float _dashForceAddition = 2f;

    float _epsilon = 0.01f;
    Vector3 _originalSize = new Vector3(1f, 1f, 1f);
    float _originalMoveSpeed = 10f;
    float _originalJumpForce = 22f;
    float _originalDashForce = 12f;

    // Dust Changing variables
    [Header("Dust Values")]
    [SerializeField] float _dust = 100f;
    [SerializeField] float _maxDust = 100f;
    [SerializeField] float[] _dustLevels;

    Vector2 _zeroVelocity = Vector3.zero;
    Vector3 _zeroVector3 = Vector3.zero;

    // Abilities booleans
    bool _canJump = true;
    bool _canDash = true;
    bool _doDash = false;
    bool _isDashing = false;
    bool _doJump = false;
    bool _growing = false;
    bool _grounded = false;
    bool _isCoyoteTime = false;
    bool Dead = false;

    // Platform variables
    private Transform _originalParent = null;

    private DropDownPlatform _currentDropDownPlatform = null;
    private RigidBodyRideable _oldRidable = null;
    private Transform _oldMovingPlatform = null;

    // Physics
    [Header("Physics")]
    Rigidbody2D _thisRigidbody;
    float _standardGravity;
    Collider2D _thisCollider;
    [SerializeField] LayerMask _environmentLayer;

    // Input
    float _horizontalInput = 0;
    float _verticalInput = 0;
    //Vector2 targetVelocity = Vector2.zero;
    Vector2 _dashDirection = Vector2.zero;
    Vector2 _jumpForceVector = Vector2.zero;


    // SFX
    PlayerSFXController _sfx;

    private void Awake()
    {
        _thisRigidbody = GetComponent<Rigidbody2D>();
        _standardGravity = _thisRigidbody.gravityScale;
        _thisCollider = GetComponent<Collider2D>();
        _mainCamera = FindObjectOfType<Camera>();
        _originalSize = transform.localScale;
        _originalMoveSpeed = _moveSpeed;
        _originalJumpForce = _jumpForce;
        _originalDashForce = _dashForce;

        _sfx = gameObject.GetComponentInChildren<PlayerSFXController>();
    }

    void Update()
    {
        gatherInput();
        //updateFriction(); //TODO: Ensure it doesnt flipflop
        updateDustSize();
        updateTimers();
    }

    void updateTimers()
    {
        _lastTimeGrounded += Time.deltaTime;

        if (!_isDashing)
            _lastTimeDashed += Time.deltaTime;
    }

    void FixedUpdate()
    {
        checkGrounded();
        if (!_isDashing)
        {
            Move();
        }

        //     _thisRigidbody.velocity = Vector2.SmoothDamp(_thisRigidbody.velocity, targetVelocity, ref _zeroVelocity, _moveSmoothing);
        if (_doJump && _canJump && _grounded)
        {
            //_doJump = false;
            Jump();
        }
        if (_doDash && _canDash && _lastTimeDashed > _dashCooldown)
        {
            //_doDash = false;
            StartCoroutine(Dash());
        }
        _doJump = false;
        _doDash = false;
    }


    private void gatherInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            _doJump = true;
        }
        if (Input.GetButtonDown("Dash"))
        {
            _doDash = true;
        }
        if (Input.GetButtonDown("Interact"))
        {
            sendInteract();
        }
    }

    private void checkGrounded()
    {
        // Raycast to check if the player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f * transform.localScale.y, _environmentLayer);
        Debug.DrawRay(transform.position, Vector2.down, Color.red); // Draw the raycast

        _grounded = (hit.collider != null) && !(hit.collider == null && _lastTimeGrounded < _coyoteTime);

        if (_grounded)
        {
            _lastTimeGrounded = 0f;
            SetCanJump(true);
            SetCanDash(true);
        }

        _currentDropDownPlatform = hit.collider?.GetComponentInChildren<DropDownPlatform>();
        // Handle Ridable Calculations
        RigidBodyRideable currentRidable = hit.collider?.GetComponentInParent<RigidBodyRideable>();
        if (currentRidable != _oldRidable)
        {
            _oldRidable?.RemoveRider(this);
            currentRidable?.SetRider(this);
        }
        _oldRidable = currentRidable;
    }

    void Move()
    {
        float _targetSpeed = _horizontalInput * _moveSpeed;
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
    }


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

        if (Mathf.Abs(_horizontalInput) == 0f)
        {
            Vector2 _addFrictionVector = Mathf.Sign(_thisRigidbody.velocity.x) * new Vector2(_friction * Time.deltaTime, 0);

            _thisRigidbody.AddForce(_addFrictionVector, ForceMode2D.Impulse);

        }
    }


    void Jump()
    {
        if (_currentDropDownPlatform != null && _verticalInput < 0f)
        {
            _currentDropDownPlatform.DropDown(_thisCollider);
        }
        else
        {
            //_thisRigidbody.velocity = new Vector2(_thisRigidbody.velocity.x, 0f);
            _thisRigidbody.velocity = Vector2.right * _thisRigidbody.velocity;
            _jumpForceVector = new Vector2(0f, _jumpForce);
            _thisRigidbody.AddForce(_jumpForceVector, ForceMode2D.Impulse);
        }
        SetCanJump(false);
        _sfx.PlaySFX(PlayerSFXController.SFX.Jump);
    }

    // Starts a timer for _coyoteTime seconds, once finished it will set grounded to false
    IEnumerator startCoyoteTime()
    {
        _isCoyoteTime = true;
        _grounded = true;
        yield return new WaitForSeconds(_coyoteTime);
        _isCoyoteTime = false;
        _grounded = false;
    }


    // Dashes for "_dashTime" seconds constantly. Uses AddForce.
    IEnumerator Dash()
    {
        _isDashing = true;
        _lastTimeDashed = 0f;


        Vector2 mousePos = Input.mousePosition;
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        _dashDirection = (mousePos - playerPos).normalized;

        _thisRigidbody.gravityScale = 0f;
        _thisRigidbody.velocity = Vector2.zero;

        _thisRigidbody.velocity = new Vector2(_dashDirection.x * _dashForce, _dashDirection.y * _dashForce);
        yield return new WaitForSeconds(_dashTime);
        _thisRigidbody.gravityScale = _standardGravity;
        _thisRigidbody.velocity = Vector2.zero;

        _lastTimeDashed = 0f;
        SetCanDash(false);
        _isDashing = false;

    }

    //// DUST FUNCTIONS

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
        if(_newDustSize != _bunnySize)
        {
            ChangeSize(_newDustSize);
        }
    }

    public void ChangeSize(int newSize)
    {
        if (_growing == true || Dead)
            return;
        Vector3 targetScale = _originalSize * Mathf.Pow(_bunnySizeScalar, newSize - 1);
        _bunnySize = newSize;

        SetMoveSpeed(_originalMoveSpeed - _moveSpeedAddition * (newSize - 1));
        SetJumpForce(_originalJumpForce + _jumpForceAddition * (newSize - 1));
        SetDashForce(_originalDashForce + _dashForceAddition * (newSize - 1));

        StartCoroutine(GrowDamp(targetScale));
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
    }


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

    public Rigidbody2D GetRigidbody2D()
    {
        return _thisRigidbody;
    }

    public void SetMoveSpeed(float _newMoveSpeed)
    {
        _moveSpeed = _newMoveSpeed;
    }
    public void SetJumpForce(float _newJumpForce)
    {
        _jumpForce = _newJumpForce;
    }
    public void SetDashForce(float _newDashForce)
    {
        _dashForce = _newDashForce;
    }

    public int GetSize()
    {
        return _bunnySize;
    }

    public void SetCanJump(bool value)
    {
        _canJump = value;
    }

    public void SetCanDash(bool value)
    {
        _canDash = value;
    }

    public void SetParent(Transform newParent)
    {
        //_originalParent = transform.parent;
        transform.parent = newParent;
    }

    public void ResetParent()
    {
        transform.parent = _originalParent;

    }

    public void SetDust(float dust)
    {
        _dust = dust;
    }

    public float GetDust()
    {
        return _dust;
    }

    public bool MaxedOutDust()
    {
        return _dust >= _maxDust;
    }

    public void AddDust(float scalar)
    {
        _dust += scalar;
        if (_dust > _maxDust)
        {
            _dust = _maxDust;
        }
    }
    public void RemoveDust(float scalar)
    {
        if (_dust - scalar < 0 && !Dead)
        {
            Die();
            return;
        }
        _dust -= scalar;

    }

    public void Die()
    {
        Dead = true; 
        Debug.Log("You died");
    }

    public PlayerSFXController GetSFX(){
        return _sfx;
    }

}
