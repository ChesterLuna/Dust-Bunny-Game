using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Camera _mainCamera;

    // Movement variables
    [SerializeField] float _moveSpeed = 500f;
    [SerializeField] float _moveSmoothing = 0.05f;

    // Abilities variables
    [SerializeField] float _jumpForce = 22f;
    [SerializeField] float _dashForce = 22f;
    [SerializeField] float _dashTime = 1f;

    // Size Changing variables
    [SerializeField] int _bunnySize = 1;
    [SerializeField] float _bunnySizeScalar = 1.5f;
    [SerializeField] float _scaleSpeed = 1.6f;
    float _epsilon = 0.01f;
    Vector3 _originalSize = new Vector3(1f, 1f, 1f);


    Vector2 _zeroVelocity = Vector3.zero;
    Vector3 _zeroVector3 = Vector3.zero;

    bool _canJump = true;
    bool _canDash = true;
    bool _doDash = false;
    bool _isDashing = false;
    bool _doJump = false;
    bool _growing = false;
    bool _grounded = false;
    DropDownPlatform _currentDropDownPlatform = null;
    Transform _oldMovingPlatform = null;

    Rigidbody2D _thisRigidbody;
    Collider2D _thisCollider;

    [SerializeField] LayerMask _environmentLayer;

    PlayerSFXController _sfx;
    private Transform _originalParent;

    float _horizontalInput = 0;
    float _verticalInput = 0;
    //Vector2 targetVelocity = Vector2.zero;
    Vector2 _dashDirection = Vector2.zero;
    Vector2 _jumpForceVector = Vector2.zero;

    private void Awake()
    {
        _thisRigidbody = GetComponent<Rigidbody2D>();
        _thisCollider = GetComponent<Collider2D>();
        _mainCamera = FindObjectOfType<Camera>();
        _originalSize = transform.localScale;
        _sfx = gameObject.GetComponentInChildren<PlayerSFXController>();
    }

    void Update()
    {
        gatherInput();
    }

    void FixedUpdate()
    {
        checkGrounded();
        Move();
        //     _thisRigidbody.velocity = Vector2.SmoothDamp(_thisRigidbody.velocity, targetVelocity, ref _zeroVelocity, _moveSmoothing);
        if (_doJump && _canJump)
        {
            _doJump = false;
            Jump();
        }
        if (_doDash && _canDash)
        {
            _doDash = false;
            StartCoroutine(Dash());
        }

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, _environmentLayer);
        Debug.DrawRay(transform.position, Vector2.down, Color.red); // Draw the raycast

        if (hit.collider != null)
        {
            _grounded = true;
            _currentDropDownPlatform = hit.collider.GetComponentInChildren<DropDownPlatform>();
            Transform currentMovingPlatform = hit.collider.GetComponentInParent<MovingPlatform>()?.transform;
            if (currentMovingPlatform != null)
            {
                if (currentMovingPlatform != _oldMovingPlatform)
                {
                    if (_oldMovingPlatform != null)
                    {
                        resetParent();
                    }
                    setParent(currentMovingPlatform);
                }
            }
            else
            {
                resetParent();
            }
            _oldMovingPlatform = currentMovingPlatform;
        }
        else
        {
            _grounded = false;
            _currentDropDownPlatform = null;
            if (_oldMovingPlatform != null)
            {
                resetParent();
                _oldMovingPlatform = null;
            }
        }
    }

    void Move()
    {
        float horizontalMovement = _horizontalInput * _moveSpeed;
        Vector2 targetVelocity = new Vector2(horizontalMovement * Time.fixedDeltaTime, _thisRigidbody.velocity.y);
        // damp movement, smooth damp
        if (_isDashing)
        {
            //_thisRigidbody.velocity = Vector2.SmoothDamp(_thisRigidbody.velocity, new Vector2(_thisRigidbody.velocity.x + horizontalMovement, _thisRigidbody.velocity.y), ref _zeroVelocity, _moveSmoothing);
            Vector2 newTargetVelocity = new Vector2(_thisRigidbody.velocity.x + horizontalMovement * Time.fixedDeltaTime, _thisRigidbody.velocity.y);
            _thisRigidbody.velocity = Vector2.SmoothDamp(_thisRigidbody.velocity, newTargetVelocity, ref _zeroVelocity, _moveSmoothing);
        }
        else
        {
            _thisRigidbody.velocity = Vector2.SmoothDamp(_thisRigidbody.velocity, targetVelocity, ref _zeroVelocity, _moveSmoothing);
            //_thisRigidbody.velocity = targetVelocity;
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
            _thisRigidbody.velocity = new Vector2(_thisRigidbody.velocity.x, 0f);
            _jumpForceVector = new Vector2(0f, _jumpForce);
            _thisRigidbody.AddForce(_jumpForceVector, ForceMode2D.Impulse);
        }
        _sfx.PlaySFX(PlayerSFXController.SFX.Jump);


    }



    // Dashes for "_dashTime" seconds constantly. Uses AddForce.
    IEnumerator Dash()
    {
        _isDashing = true;

        Vector2 mousePos = Input.mousePosition;//_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        _dashDirection = (mousePos - playerPos).normalized;

        float gravity = _thisRigidbody.gravityScale;
        _thisRigidbody.gravityScale = 0f;

        _thisRigidbody.AddForce(new Vector2(_dashDirection.x * _dashForce, _dashDirection.y * _dashForce), ForceMode2D.Impulse);
        yield return new WaitForSeconds(_dashTime);
        _thisRigidbody.gravityScale = gravity;

        _isDashing = false;
    }

    public void ChangeSize(int newSize)
    {
        if (_growing == true)
            return;
        Vector3 targetScale = _originalSize * Mathf.Pow(_bunnySizeScalar, newSize - 1);
        _bunnySize = newSize;

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


    public Rigidbody2D GetRigidbody2D()
    {
        return _thisRigidbody;
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


    void setParent(Transform newParent)
    {
        _originalParent = transform.parent;
        transform.parent = newParent;
    }

    void resetParent()
    {
        transform.parent = _originalParent;
    }

}
