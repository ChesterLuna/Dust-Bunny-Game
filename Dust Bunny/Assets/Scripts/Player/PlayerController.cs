using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour, IPlayerController, IPhysicsObject
{
    #region References

    private BoxCollider2D _collider;
    private CapsuleCollider2D _airborneCollider;
    private ConstantForce2D _constantForce;
    private Rigidbody2D _rb;

    #endregion

    [SerializeField] GameObject _actionIndicator;
    #region Camera
    // Camera
    [Header("Camera")]
    [SerializeField] private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;
    #endregion

    #region Interface
    [field: SerializeField] private PlayerStats[] _allStats = new PlayerStats[3];

    [field: SerializeField] public PlayerStats Stats { get; private set; }
    public ControllerState State { get; private set; }
    public event Action<JumpType> Jumped;
    public event Action<bool, float> GroundedChanged;
    public event Action<bool, Vector2> DashChanged;
    public event Action<bool> WallGrabChanged;
    public event Action SizeChanged;
    public event Action<float, bool> UsedDust;
    public event Action<Vector2> Repositioned;
    public event Action<bool, bool> ToggledPlayer;

    public bool Active { get; private set; } = true;
    public bool ActiveDialogue { get; private set; } = true;
    public Vector2 Up { get; private set; }
    public Vector2 Right { get; private set; }
    public Vector2 Input => _frameInput.Move;
    public Vector2 GroundNormal { get; private set; }
    public Vector2 Velocity { get; private set; }
    public int WallDirection { get; private set; }
    public bool ClimbingLadder { get; private set; }
    public PlayerStates PlayerState { get; set; } = PlayerStates.Playing;

    public void AddFrameForce(Vector2 force, bool resetVelocity = false)
    {
        if (resetVelocity) SetVelocity(Vector2.zero);
        _forceToApplyThisFrame += force;
    } // end AddFrameForce

    public void LoadState(ControllerState state)
    {
        RepositionImmediately(state.Position);
        _rb.rotation = state.Rotation;
        SetVelocity(state.Velocity);

        if (state.Grounded) ToggleGrounded(true);
    } // end LoadState

    public void RepositionImmediately(Vector2 position, bool resetVelocity = false)
    {
        _rb.position = position;
        if (resetVelocity) SetVelocity(Vector2.zero);
        Repositioned?.Invoke(position);
    } // end RepositionImmediately

    public void TogglePlayer(bool on, bool dead = false, bool changeAnimation = true)
    {
        Active = on;
        _rb.isKinematic = !on;
        if (changeAnimation) ToggledPlayer?.Invoke(on, dead);
    } // end TogglePlayer
    public void TogglePlayerDialogue(bool on, bool dead = false, bool changeAnimation = true)
    {
        ActiveDialogue = on;
        _rb.isKinematic = !on;
        if (changeAnimation) ToggledPlayer?.Invoke(on, dead);
    } // end TogglePlayer

    #endregion

    [SerializeField] private bool _drawGizmos = true;

    #region Loop

    private float _delta, _time;

    private void Awake()
    {
        if (!TryGetComponent(out _constantForce)) _constantForce = gameObject.AddComponent<ConstantForce2D>();
        if (GameManager.instance.CheckpointDustLevel.HasValue) _currentDust = GameManager.instance.CheckpointDustLevel.Value;
        SetupCharacter();

        PhysicsSimulator.Instance.AddPlayer(this);
    } // end Awake

    private void Start()
    {
        if (GameManager.instance.CheckpointLocation.HasValue)
        {
            RepositionImmediately(GameManager.instance.CheckpointLocation.Value, true);
        }
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    } // end Start

    private void OnDestroy() => PhysicsSimulator.Instance.RemovePlayer(this);

    public void OnValidate() => SetupCharacter();

    public void TickUpdate(float delta, float time)
    {
        _delta = delta;
        _time = time;

        GatherInput();
        updateCameraYDamping();

    } // end TickUpdate

    public void TickFixedUpdate(float delta)
    {
        _delta = delta;
        if (!Active)
        {
            SetVelocity(Vector2.zero);
            return;
        }
        if (!ActiveDialogue)
        {
            SetVelocity(Vector2.up * _rb.velocity);
            RemoveTransientVelocity();

            SetFrameData();

            CalculateCollisions();
            CalculateDirection();

            CalculateInteract();
            CleanFrameData();
            CalculateExternalModifiers();

            TraceGround();
            // Move();

            return;
        }

        RemoveTransientVelocity();

        SetFrameData();

        CalculateCollisions();
        CalculateDirection();

        CalculateWalls();
        CalculateLadders();
        CalculateJump();
        CalculateDash();
        CalculateInteract();

        CalculateExternalModifiers();

        TraceGround();
        Move();

        CleanFrameData();

        SaveCharacterState();
    } // end TickFixedUpdate

    #endregion

    #region Cameras
    private void updateCameraYDamping()
    {
        // If we are falling past a certain speed threshold
        if (_rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        if (_rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    } // end updateCameraYDamping
    #endregion

    #region Setup

    private bool _cachedQueryMode, _cachedQueryTriggers;
    private GeneratedCharacterSize _character;
    private const float GRAVITY_SCALE = 1;

    private void SetupCharacter(ColliderMode mode = ColliderMode.Airborne)
    {
        Stats = _allStats[DustLevelIndex(_currentDust)];
        CameraManager.instance?.SetOrthographicSize(Stats.CameraOrthographicSize);

        _character = Stats.CharacterSize.GenerateCharacterSize();
        _cachedQueryMode = Physics2D.queriesStartInColliders;

        _wallDetectionBounds = new Bounds(
            new Vector3(0, _character.Height / 2),
            new Vector3(_character.StandingColliderSize.x + CharacterSize.COLLIDER_EDGE_RADIUS * 2 + Stats.WallDetectorRange, _character.Height - 0.1f));

        _rb = GetComponent<Rigidbody2D>();
        _rb.hideFlags = HideFlags.NotEditable;

        // Primary collider
        _collider = GetComponent<BoxCollider2D>();
        _collider.edgeRadius = CharacterSize.COLLIDER_EDGE_RADIUS;
        _collider.hideFlags = HideFlags.NotEditable;
        _collider.sharedMaterial = _rb.sharedMaterial;
        _collider.enabled = true;

        // Airborne collider
        _airborneCollider = GetComponent<CapsuleCollider2D>();
        _airborneCollider.hideFlags = HideFlags.NotEditable;
        _airborneCollider.size = new Vector2(_character.Width - SKIN_WIDTH * 2, _character.Height - SKIN_WIDTH * 2);
        _airborneCollider.offset = new Vector2(0, _character.Height / 2);
        _airborneCollider.sharedMaterial = _rb.sharedMaterial;

        SetColliderMode(mode);
        SizeChanged?.Invoke();
    } // end SetupCharacter

    #endregion

    #region Input

    private FrameInput _frameInput;

    private void GatherInput()
    {
        _frameInput = UserInput.instance.Gather();


        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        if (_frameInput.DashDown)
        {
            _dashToConsume = true;
        }

        if (_frameInput.InteractDown)
        {
            _interactToConsume = true;
        }
    } // end GatherInput

    #endregion

    #region Frame Data

    private bool _hasInputThisFrame;
    private Vector2 _trimmedFrameVelocity;
    private Vector2 _framePosition;
    private Bounds _wallDetectionBounds;

    private void SetFrameData()
    {
        var rot = _rb.rotation * Mathf.Deg2Rad;
        Up = new Vector2(-Mathf.Sin(rot), Mathf.Cos(rot));
        Vector2 previousRight = Right;
        Right = new Vector2(Up.y, -Up.x);
        if (previousRight != Right) _cameraFollowObject.CallTurn(); // Update Camera
        _framePosition = _rb.position;

        _hasInputThisFrame = _frameInput.Move.x != 0;

        Velocity = _rb.velocity;
        _trimmedFrameVelocity = new Vector2(Velocity.x, 0);
    } // end SetFrameData

    private void RemoveTransientVelocity()
    {
        var currentVelocity = _rb.velocity;
        var velocityBeforeReduction = currentVelocity;

        currentVelocity -= _totalTransientVelocityAppliedLastFrame;
        SetVelocity(currentVelocity);

        _frameTransientVelocity = Vector2.zero;
        _totalTransientVelocityAppliedLastFrame = Vector2.zero;

        // If flung into a wall, dissolve the decay
        // Replace this entire section with Boubourriquet's solution
        var decay = Stats.Friction * Stats.AirFrictionMultiplier * Stats.ExternalVelocityDecayRate;
        if ((velocityBeforeReduction.x < 0 && _decayingTransientVelocity.x < velocityBeforeReduction.x) ||
            (velocityBeforeReduction.x > 0 && _decayingTransientVelocity.x > velocityBeforeReduction.x) ||
            (velocityBeforeReduction.y < 0 && _decayingTransientVelocity.y < velocityBeforeReduction.y) ||
            (velocityBeforeReduction.y > 0 && _decayingTransientVelocity.y > velocityBeforeReduction.y)) decay *= 5;

        _decayingTransientVelocity = Vector2.MoveTowards(_decayingTransientVelocity, Vector2.zero, decay * _delta);

        _immediateMove = Vector2.zero;
    } // end RemoveTransientVelocity

    private void CleanFrameData()
    {
        _jumpToConsume = false;
        _dashToConsume = false;
        _interactToConsume = false;
        _forceToApplyThisFrame = Vector2.zero;
        _lastFrameY = Velocity.y;
    } // end CleanFrameData

    #endregion

    #region Collisions

    private const float SKIN_WIDTH = 0.02f;
    private const int RAY_SIDE_COUNT = 5;
    private RaycastHit2D _groundHit;
    private bool _grounded;
    private float _currentStepDownLength;
    private float GrounderLength => _character.StepHeight + SKIN_WIDTH;

    private Vector2 RayPoint => _framePosition + Up * (_character.StepHeight + SKIN_WIDTH);

    private void CalculateCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Is the middle ray good?
        var isGroundedThisFrame = PerformRay(RayPoint);

        // If not, zigzag rays from the center outward until we find a hit
        if (!isGroundedThisFrame)
        {
            foreach (var offset in GenerateRayOffsets())
            {
                isGroundedThisFrame = PerformRay(RayPoint + Right * offset) || PerformRay(RayPoint - Right * offset);
                if (isGroundedThisFrame) break;
            }
        }

        if (isGroundedThisFrame && !_grounded) ToggleGrounded(true);
        else if (!isGroundedThisFrame && _grounded) ToggleGrounded(false);

        Physics2D.queriesStartInColliders = _cachedQueryMode;

        bool PerformRay(Vector2 point)
        {
            _groundHit = Physics2D.Raycast(point, -Up, GrounderLength + _currentStepDownLength, Stats.CollisionLayers);
            if (!_groundHit) return false;

            if (Vector2.Angle(_groundHit.normal, Up) > Stats.MaxWalkableSlope)
            {
                return false;
            }

            return true;
        }
    } // end CalculateCollisions

    private IEnumerable<float> GenerateRayOffsets()
    {
        var extent = _character.StandingColliderSize.x / 2 - _character.RayInset;
        var offsetAmount = extent / RAY_SIDE_COUNT;
        for (var i = 1; i < RAY_SIDE_COUNT + 1; i++)
        {
            yield return offsetAmount * i;
        }
    } // end GenerateRayOffsets

    private void ToggleGrounded(bool grounded)
    {
        _grounded = grounded;
        if (grounded)
        {
            GroundedChanged?.Invoke(true, _lastFrameY);
            _rb.gravityScale = 0;
            SetVelocity(_trimmedFrameVelocity);
            _constantForce.force = Vector2.zero;
            _currentStepDownLength = _character.StepHeight;
            ResetDashes();
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            ResetAirJumps();
            SetColliderMode(ColliderMode.Standard);
        }
        else
        {
            GroundedChanged?.Invoke(false, 0);
            _timeLeftGrounded = _time;
            _rb.gravityScale = GRAVITY_SCALE;
            SetColliderMode(ColliderMode.Airborne);
        }
    } // end ToggleGrounded

    private void SetColliderMode(ColliderMode mode)
    {
        _airborneCollider.enabled = mode == ColliderMode.Airborne;

        switch (mode)
        {
            case ColliderMode.Standard:
                _collider.size = _character.StandingColliderSize;
                _collider.offset = _character.StandingColliderCenter;
                break;
            case ColliderMode.Airborne:
                break;
        }
    } // end SetColliderMode

    private enum ColliderMode
    {
        Standard,
        Airborne
    } // end enum ColliderMode

    #endregion

    #region Direction

    private Vector2 _frameDirection;

    private void CalculateDirection()
    {
        _frameDirection = new Vector2(_frameInput.Move.x, 0);

        if (_grounded)
        {
            GroundNormal = _groundHit.normal;
            var angle = Vector2.Angle(GroundNormal, Up);
            if (angle < Stats.MaxWalkableSlope) _frameDirection.y = _frameDirection.x * -GroundNormal.x / GroundNormal.y;
        }

        _frameDirection = _frameDirection.normalized;
    } // end CalculateDirection

    #endregion

    #region Walls

    private const float WALL_REATTACH_COOLDOWN = 0.2f;

    private float _wallJumpInputNerfPoint;
    private int _wallDirectionForJump;
    private bool _isOnWall;
    private float _timeLeftWall;
    private float _currentWallSpeedVel;
    private float _canGrabWallAfter;
    private int _wallDirThisFrame;

    private bool HorizontalInputPressed => Mathf.Abs(_frameInput.Move.x) > Stats.HorizontalDeadZoneThreshold;
    private bool IsPushingAgainstWall => HorizontalInputPressed && (int)Mathf.Sign(_frameDirection.x) == _wallDirThisFrame;

    private void CalculateWalls()
    {
        if (!Stats.AllowWalls) return;

        var rayDir = _isOnWall ? WallDirection : _frameDirection.x;
        var hasHitWall = DetectWallCast(rayDir);

        _wallDirThisFrame = hasHitWall ? (int)rayDir : 0;

        if (!_isOnWall && ShouldStickToWall() && _time > _canGrabWallAfter && Velocity.y < 0) ToggleOnWall(true);
        else if (_isOnWall && !ShouldStickToWall()) ToggleOnWall(false);

        // If we're not grabbing a wall, let's check if we're against one for wall-jumping purposes
        if (!_isOnWall)
        {
            if (DetectWallCast(-1)) _wallDirThisFrame = -1;
            else if (DetectWallCast(1)) _wallDirThisFrame = 1;
        }

        bool ShouldStickToWall()
        {
            if (_wallDirThisFrame == 0 || _grounded) return false;

            if (HorizontalInputPressed && !IsPushingAgainstWall) return false; // If pushing away
            return !Stats.RequireInputPush || (IsPushingAgainstWall);
        }
    } // end CalculateWalls

    private bool DetectWallCast(float dir)
    {
        return Physics2D.BoxCast(_framePosition + (Vector2)_wallDetectionBounds.center, new Vector2(_character.StandingColliderSize.x - SKIN_WIDTH, _wallDetectionBounds.size.y), 0, new Vector2(dir, 0), Stats.WallDetectorRange,
            Stats.ClimbableLayer);
    } // end DetectWallCast

    private void ToggleOnWall(bool on)
    {
        _isOnWall = on;

        if (on)
        {
            _decayingTransientVelocity = Vector2.zero;
            _bufferedJumpUsable = true;
            _wallJumpCoyoteUsable = true;
            WallDirection = _wallDirThisFrame;
        }
        else
        {
            _timeLeftWall = _time;
            _canGrabWallAfter = _time + WALL_REATTACH_COOLDOWN;
            _rb.gravityScale = GRAVITY_SCALE;
            WallDirection = 0;
            if (Velocity.y > 0)
            {
                AddFrameForce(new Vector2(0, Stats.WallPopForce), true);
            }

            ResetAirJumps(); // so that we can air jump even if we didn't leave via a wall jump
        }

        WallGrabChanged?.Invoke(on);
    } // end ToggleOnWall

    #endregion

    #region Ladders

    private bool CanEnterLadder => _ladderHit && _time > _timeLeftLadder + Stats.LadderCooldownTime;
    private bool ShouldMountLadder => Stats.AutoAttachToLadders || _frameInput.Move.y > Stats.VerticalDeadZoneThreshold || (!_grounded && _frameInput.Move.y < -Stats.VerticalDeadZoneThreshold);
    private bool ShouldDismountLadder => !Stats.AutoAttachToLadders && _grounded && _frameInput.Move.y < -Stats.VerticalDeadZoneThreshold;

    private float _timeLeftLadder;
    private Collider2D _ladderHit;
    private float _ladderSnapVel;

    private void CalculateLadders()
    {
        if (!Stats.AllowLadders) return;

        Physics2D.queriesHitTriggers = true; // Ladders are set to Trigger
        _ladderHit = Physics2D.OverlapBox(_framePosition + (Vector2)_wallDetectionBounds.center, _wallDetectionBounds.size, 0, Stats.LadderLayer);

        Physics2D.queriesHitTriggers = _cachedQueryTriggers;

        if (!ClimbingLadder && CanEnterLadder && ShouldMountLadder) ToggleClimbingLadder(true);
        else if (ClimbingLadder && (!_ladderHit || ShouldDismountLadder)) ToggleClimbingLadder(false);
    } // end CalculateLadders

    private void ToggleClimbingLadder(bool on)
    {
        if (ClimbingLadder == on) return;
        if (on)
        {
            SetVelocity(Vector2.zero);
            _rb.gravityScale = 0;
            _ladderSnapVel = 0; // reset damping velocity for consistency
        }
        else
        {
            if (_ladderHit) _timeLeftLadder = _time; // to prevent immediately re-mounting ladder
            if (_frameInput.Move.y > 0)
            {
                AddFrameForce(new Vector2(0, Stats.LadderPopForce));
            }

            _rb.gravityScale = GRAVITY_SCALE;
        }

        ClimbingLadder = on;
        ResetAirJumps();
    } // end ToggleClimbingLadder

    #endregion

    #region Jump

    private const float JUMP_CLEARANCE_TIME = 0.25f;
    private bool IsWithinJumpClearance => _lastJumpExecutedTime + JUMP_CLEARANCE_TIME > _time;
    private float _lastJumpExecutedTime;
    private bool _bufferedJumpUsable;
    private bool _jumpToConsume;
    private float _timeJumpWasPressed;
    private Vector2 _forceToApplyThisFrame;
    private bool _endedJumpEarly;
    private float _endedJumpForce;
    private int _totalAirJumpsRemaining;
    private int _freeAirJumpsRemaining;
    private bool _wallJumpCoyoteUsable;
    private bool _coyoteUsable;
    private float _timeLeftGrounded;
    private float _returnWallInputLossAfter;
    private DropDownPlatform _currentDropDownPlatform = null;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + Stats.BufferedJumpTime && !IsWithinJumpClearance;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _timeLeftGrounded + Stats.CoyoteTime;
    private bool CanAirJump => !_grounded && _totalAirJumpsRemaining > 0 && (_freeAirJumpsRemaining > 0 || _currentDust + Stats.AirJumpCost > 0);
    private bool CanWallJump => !_grounded && (_isOnWall || _wallDirThisFrame != 0) || (_wallJumpCoyoteUsable && _time < _timeLeftWall + Stats.WallCoyoteTime);

    private void CalculateJump()
    {
        if (_jumpToConsume || HasBufferedJump)
        {
            if (CanWallJump)
            {
                ExecuteJump(JumpType.WallJump);
            }
            else if (_currentDropDownPlatform != null && _frameInput.Move.y < 0f)
            {
                ExecuteJump(JumpType.PlatformJumpDrop);
            }
            else if (_grounded || ClimbingLadder)
            {
                ExecuteJump(JumpType.Jump);
            }
            else if (CanUseCoyote)
            {
                ExecuteJump(JumpType.Coyote);
            }
            else if (CanAirJump)
            {
                ExecuteJump(JumpType.AirJump);
            }
        }

        if ((!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && Velocity.y > 0) || Velocity.y < 0) _endedJumpEarly = true; // Early end detection


        if (_time > _returnWallInputLossAfter) _wallJumpInputNerfPoint = Mathf.MoveTowards(_wallJumpInputNerfPoint, 1, _delta / Stats.WallJumpInputLossReturnTime);
    } // end CalculateJump

    private void ExecuteJump(JumpType jumpType)
    {
        SetVelocity(_trimmedFrameVelocity);
        _endedJumpEarly = false;
        _bufferedJumpUsable = false;
        _lastJumpExecutedTime = _time;
        _currentStepDownLength = 0;
        if (ClimbingLadder) ToggleClimbingLadder(false);

        if (jumpType is JumpType.Jump or JumpType.Coyote)
        {
            _coyoteUsable = false;
            AddFrameForce(new Vector2(0, Stats.JumpPower));
        }
        else if (jumpType is JumpType.AirJump)
        {
            _totalAirJumpsRemaining--;
            if (_freeAirJumpsRemaining > 0) _freeAirJumpsRemaining--;
            else
            {
                ChangeDust(Stats.AirJumpCost, false);
            }
            AddFrameForce(new Vector2(0, Stats.JumpPower));
        }
        else if (jumpType is JumpType.WallJump)
        {
            ToggleOnWall(false);

            _wallJumpCoyoteUsable = false;
            _wallJumpInputNerfPoint = 0;
            _returnWallInputLossAfter = _time + Stats.WallJumpTotalInputLossTime;
            _wallDirectionForJump = _wallDirThisFrame;
            if (_isOnWall || IsPushingAgainstWall)
            {
                AddFrameForce(new Vector2(-_wallDirThisFrame, 1) * Stats.WallJumpPower);
            }
            else
            {
                AddFrameForce(new Vector2(-_wallDirThisFrame, 1) * Stats.WallPushPower);
            }
        }
        else if (jumpType is JumpType.PlatformJumpDrop)
        {
            _currentDropDownPlatform.DropDown(_collider);
        }

        Jumped?.Invoke(jumpType);
    } // end ExecuteJump

    private void ResetAirJumps()
    {
        _totalAirJumpsRemaining = Stats.MaxAirJumps;
        _freeAirJumpsRemaining = Stats.AirJumpBeforeCost;
    }
    #endregion

    #region Dash

    private bool _dashToConsume;
    private bool _canDash;
    private Vector2 _dashVel;
    private bool _dashing;
    private float _startedDashing;
    private float _nextDashTime;
    private int _totalDashesRemaining;
    private int _freeDashesRemaining;

    public bool CanDash(){
        return Stats.AllowDash && _totalDashesRemaining > 0 && (_canDash || _freeDashesRemaining > 0 || _currentDust + Stats.DashCost > 0) && _time > _nextDashTime;
    }

    private void CalculateDash()
    {
        if (!Stats.AllowDash) return;

        if (_dashToConsume && CanDash())
        {
            // Handle the dash
            Vector2 dir;
            if (UserInput.instance.UseMouseForDash)
            {
                Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
                dir = (_frameInput.DashDirection - playerPos).normalized;
            }
            else
            {
                dir = new Vector2(_frameInput.Move.x, _frameInput.Move.y).normalized;

            }
            if (dir == Vector2.zero) return;

            _dashVel = dir * Stats.DashVelocity;
            _dashing = true;
            _canDash = false;
            _startedDashing = _time;
            _nextDashTime = _time + Stats.DashCooldown;
            DashChanged?.Invoke(true, dir);

            // Handle Costs
            if (_freeDashesRemaining > 0) _freeDashesRemaining--;
            else
            {
                ChangeDust(Stats.DashCost, false);
            }
            _totalDashesRemaining--;

        }

        if (_dashing)
        {
            if (_time > _startedDashing + Stats.DashDuration)
            {
                _dashing = false;
                DashChanged?.Invoke(false, Vector2.zero);

                SetVelocity(new Vector2(Velocity.x * Stats.DashEndHorizontalMultiplier, Velocity.y));
                if (_grounded)
                {
                    ResetDashes();
                }
            }
        }
    } // end CalculateDash

    private void ResetDashes()
    {
        _canDash = true;
        _totalDashesRemaining = Stats.MaxDashes;
        _freeDashesRemaining = Stats.DashesBeforeCost;
    } // end ResetDashes

    #endregion

    #region Interact
    private bool _interactToConsume;
    private void CalculateInteract()
    {
        if (!_interactToConsume) return;
        List<Collider2D> results = new List<Collider2D>();
        Collider2D activeCollider = _collider.isActiveAndEnabled ? _collider : _airborneCollider;

        Physics2D.queriesHitTriggers = true;
        activeCollider.OverlapCollider(new ContactFilter2D().NoFilter(), results);
        Physics2D.queriesHitTriggers = _cachedQueryTriggers;
        foreach (Collider2D collider in results)
        {
            collider.gameObject.GetComponent<IInteractable>()?.Interact();
        }
    } // end CalculateInteract
    #endregion

    public Vector2 GetColliderPosition()
    {
        Collider2D activeCollider = _collider.isActiveAndEnabled ? _collider : _airborneCollider;
        return activeCollider.bounds.center;
    }

    #region Move

    private Vector2 _frameTransientVelocity;
    private Vector2 _immediateMove;
    private Vector2 _decayingTransientVelocity;
    private Vector2 _totalTransientVelocityAppliedLastFrame;
    private Vector2 _frameSpeedModifier, _currentFrameSpeedModifier = Vector2.one;
    private const float SLOPE_ANGLE_FOR_EXACT_MOVEMENT = 0.7f;
    private IPhysicsMover _lastPlatform;
    private float _lastFrameY;

    private void TraceGround()
    {
        IPhysicsMover currentPlatform = null;

        if (_grounded && !IsWithinJumpClearance)
        {
            // Use transient velocity to keep grounded. Move position is not interpolated
            var distanceFromGround = _character.StepHeight - _groundHit.distance;
            if (distanceFromGround != 0)
            {
                var requiredMove = Vector2.zero;
                requiredMove.y += distanceFromGround;
                if (Stats.PositionCorrectionMode is PositionCorrectionMode.Velocity) _frameTransientVelocity = requiredMove / _delta;
                else _immediateMove = requiredMove;
            }

            if (_groundHit.transform.TryGetComponent(out currentPlatform))
            {
                _activatedMovers.Add(currentPlatform);
            }

            _currentDropDownPlatform = _groundHit.transform.GetComponentInChildren<DropDownPlatform>();
        }

        if (_lastPlatform != currentPlatform)
        {
            // With no bounding, this is a simple contact-only platform
            // If it does have a bounding, we'll disconnect from it when the trigger exits
            if (_lastPlatform is { UsesBounding: false })
            {
                _activatedMovers.Remove(_lastPlatform);
                ApplyMoverExitVelocity(_lastPlatform);
            }

            _lastPlatform = currentPlatform;
        }

        // Handle platforms
        foreach (var platform in _activatedMovers)
        {
            // Don't apply if we're next to it
            if (_framePosition.y < platform.FramePosition.y - SKIN_WIDTH) continue;
            _frameTransientVelocity += platform.FramePositionDelta / _delta;
        }
    } // end TraceGround

    private void ApplyMoverExitVelocity(IPhysicsMover mover)
    {
        var platformVel = mover.TakeOffVelocity;
        if (platformVel.y < 0) platformVel.y *= Stats.NegativeYVelocityNegation;
        _decayingTransientVelocity += platformVel;
    } // end ApplyMoverExitVelocity

    private void Move()
    {
        if (_forceToApplyThisFrame != Vector2.zero)
        {
            _rb.velocity += AdditionalFrameVelocities();
            _rb.AddForce(_forceToApplyThisFrame * _rb.mass, ForceMode2D.Impulse);

            // Returning provides the crispest & most accurate jump experience
            // Required for reliable slope jumps
            return;
        }

        if (_dashing)
        {
            SetVelocity(_dashVel);
            return;
        }

        if (_isOnWall)
        {
            _constantForce.force = Vector2.zero;

            float wallVelocity;
            if (_frameInput.Move.y != 0) wallVelocity = _frameInput.Move.y * Stats.WallClimbSpeed;
            else wallVelocity = Mathf.MoveTowards(Mathf.Min(Velocity.y, 0), -Stats.WallClimbSpeed, Stats.WallFallAcceleration * _delta);

            SetVelocity(new Vector2(_rb.velocity.x, wallVelocity));
            return;
        }

        if (ClimbingLadder)
        {
            _constantForce.force = Vector2.zero;
            _rb.gravityScale = 0;

            var goalVelocity = Vector2.zero;
            goalVelocity.y = _frameInput.Move.y * (_frameInput.Move.y > 0 ? Stats.LadderClimbSpeed : Stats.LadderSlideSpeed);

            // Horizontal
            float goalX;
            if (Stats.SnapToLadders && _frameInput.Move.x == 0)
            {
                var targetX = _ladderHit.transform.position.x;
                goalX = Mathf.SmoothDamp(_framePosition.x, targetX, ref _ladderSnapVel, Stats.LadderSnapTime);
            }
            else
            {
                goalX = Mathf.MoveTowards(_framePosition.x, _framePosition.x + _frameInput.Move.x, Stats.Acceleration * Stats.LadderShimmySpeedMultiplier * _delta);
            }

            goalVelocity.x = (goalX - _framePosition.x) / _delta;

            SetVelocity(goalVelocity);

            return;
        }

        var extraForce = new Vector2(0, _grounded ? 0 : -Stats.ExtraConstantGravity * (_endedJumpEarly && Velocity.y > 0 ? Stats.EndJumpEarlyExtraForceMultiplier : 1));
        _constantForce.force = extraForce * _rb.mass;

        var targetSpeed = _hasInputThisFrame ? Stats.BaseSpeed : 0;

        var step = _hasInputThisFrame ? Stats.Acceleration : Stats.Friction;

        var xDir = _hasInputThisFrame ? _frameDirection : Velocity.normalized;

        // Quicker direction change
        if (Vector3.Dot(_trimmedFrameVelocity, _frameDirection) < 0) step *= Stats.DirectionCorrectionMultiplier;

        Vector2 newVelocity;
        step *= _delta;
        if (_grounded)
        {
            var speed = Mathf.MoveTowards(Velocity.magnitude, targetSpeed, step);

            // Blend the two approaches
            var targetVelocity = xDir * speed;

            // Calculate the new speed based on the current and target speeds
            var newSpeed = Mathf.MoveTowards(Velocity.magnitude, targetVelocity.magnitude, step);

            // TODO: Lets actually trace the ground direction automatically instead of direct
            var smoothed = Vector2.MoveTowards(Velocity, targetVelocity, step); // Smooth but potentially inaccurate
            var direct = targetVelocity.normalized * newSpeed; // Accurate but abrupt
            var slopePoint = Mathf.InverseLerp(0, SLOPE_ANGLE_FOR_EXACT_MOVEMENT, Mathf.Abs(_frameDirection.y)); // Blend factor

            // Calculate the blended velocity
            newVelocity = Vector2.Lerp(smoothed, direct, slopePoint);
        }
        else
        {
            step *= Stats.AirFrictionMultiplier;

            if (_wallJumpInputNerfPoint < 1 && (int)Mathf.Sign(xDir.x) == (int)Mathf.Sign(_wallDirectionForJump))
            {
                if (_time < _returnWallInputLossAfter) xDir.x = -_wallDirectionForJump;
                else xDir.x *= _wallJumpInputNerfPoint;
            }

            var targetX = Mathf.MoveTowards(_trimmedFrameVelocity.x, xDir.x * targetSpeed, step);
            newVelocity = new Vector2(targetX, _rb.velocity.y);
        }

        SetVelocity((newVelocity + AdditionalFrameVelocities()) * _currentFrameSpeedModifier);

        Vector2 AdditionalFrameVelocities()
        {
            if (_immediateMove.sqrMagnitude > SKIN_WIDTH)
            {
                _rb.MovePosition(_framePosition + _immediateMove);
            }
            _totalTransientVelocityAppliedLastFrame = _frameTransientVelocity + _decayingTransientVelocity;
            return _totalTransientVelocityAppliedLastFrame;
        }
    } // end Move

    public void SetVelocity(Vector2 newVel)
    {
        _rb.velocity = newVel;
        Velocity = newVel;
    } // end SetVelocity

    #endregion

    private void SaveCharacterState()
    {
        State = new ControllerState
        {
            Position = _framePosition,
            Rotation = _rb.rotation,
            Velocity = Velocity,
            Grounded = _grounded
        };
    } // end SaveCharacterState

    #region Dust & Size

    // Size Changing variables
    // [Header("Size Changing")]
    // [Range(0, 5)][SerializeField] int _bunnySize = 1;
    // [SerializeField] float _bunnySizeScalar = 1.5f;
    // Vector3 _originalSize = new Vector3(1f, 1f, 1f);

    // [SerializeField] float _scaleSpeed = 1.6f;
    // bool _growing = false;
    // Coroutine _lastDamp;
    // float _epsilon = 0.01f;


    // Dust Changing variables
    [Header("Dust Values")]
    [SerializeField] private float _currentDust = 100f;
    [HideInInspector] public float CurrentDust => _currentDust;
    [SerializeField] private float _maxDust = 100f;
    [SerializeField] private float[] _dustLevels = new float[5] { 0f, 20f, 40f, 60f, 80f };

    private int DustLevelIndex(float dustLevel)
    {
        for (int i = _dustLevels.Length - 1; i > -1; i--)
        {
            if (dustLevel >= _dustLevels[i])
            {
                return i;
            }
        }
        // Safety return
        return 0;
    } // end DustLevelIndex

    public void ChangeDust(float scalar, bool hostile)
    {
        if (scalar < 0) UsedDust?.Invoke(scalar, hostile);

        float _oldDust = _currentDust;
        _currentDust += scalar;
        if (_currentDust > _maxDust)
        {
            _currentDust = _maxDust;
        }
        else if (_currentDust <= 0)
        {
            _currentDust = 0;
            Die();
            return;
        }
        if (DustLevelIndex(_currentDust) != DustLevelIndex(_oldDust))
        {
            ColliderMode mode = _airborneCollider.enabled ? ColliderMode.Airborne : ColliderMode.Standard;
            SetupCharacter(mode);
            SizeChanged?.Invoke();
        }
    } // end ChangeDust
    #endregion

    [Header("Death")]
    [SerializeField] private Animator _deathTransition;
    [SerializeField] private float _deathTransitionTime = 1f;
    public void Die() // TODO, Update
    {
        TogglePlayer(false, true);
        PlayerState = PlayerStates.Dead;
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevel(SceneManager.GetActiveScene().name, _deathTransition, _deathTransitionTime);
        GameManager.instance.PauseGameTime();
    } // end Die

    #region External Triggers

    private const int MAX_ACTIVE_MOVERS = 5;
    private readonly HashSet<IPhysicsMover> _activatedMovers = new(MAX_ACTIVE_MOVERS);
    private readonly HashSet<ISpeedModifier> _modifiers = new();
    private Vector2 _frameSpeedModifierVelocity;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ISpeedModifier modifier)) _modifiers.Add(modifier);
        else if (other.TryGetComponent(out IPhysicsMover mover) && !mover.RequireGrounding) _activatedMovers.Add(mover);
    } // end OnTriggerEnter2D

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out ISpeedModifier modifier)) _modifiers.Remove(modifier);
        else if (other.TryGetComponent(out IPhysicsMover mover)) _activatedMovers.Remove(mover);

        other.TryGetComponent(out IInteractable interactScript);
        if (interactScript != null && interactScript.ShowIndicator)
        {
            _actionIndicator.gameObject.SetActive(false);
        }
    } // end OnTriggerExit2D

    void OnTriggerStay2D(Collider2D other)
    {
        other.TryGetComponent(out IInteractable interactScript);
        if (interactScript != null && interactScript.ShowIndicator)
        {
            _actionIndicator.gameObject.SetActive(true);
        }
    } // end OnTriggerStay2D

    private void CalculateExternalModifiers()
    {
        _frameSpeedModifier = Vector2.one;
        foreach (var modifier in _modifiers)
        {
            if ((modifier.OnGround && _grounded) || (modifier.InAir && !_grounded))
                _frameSpeedModifier += modifier.Modifier;
        }

        _currentFrameSpeedModifier = Vector2.SmoothDamp(_currentFrameSpeedModifier, _frameSpeedModifier, ref _frameSpeedModifierVelocity, 0.1f);
    } // end CalculateExternalModifiers

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;

        var pos = (Vector2)transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos + Vector2.up * _character.Height / 2, new Vector3(_character.Width, _character.Height));
        Gizmos.color = Color.magenta;

        var rayStart = pos + Vector2.up * _character.StepHeight;
        var rayDir = Vector3.down * _character.StepHeight;
        Gizmos.DrawRay(rayStart, rayDir);
        foreach (var offset in GenerateRayOffsets())
        {
            Gizmos.DrawRay(rayStart + Vector2.right * offset, rayDir);
            Gizmos.DrawRay(rayStart + Vector2.left * offset, rayDir);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos + (Vector2)_wallDetectionBounds.center, _wallDetectionBounds.size);


        Gizmos.color = Color.black;
        Gizmos.DrawRay(RayPoint, Vector3.right);
    } // end OnDrawGizmos

    #endregion
} // end class PlayerController

public enum JumpType
{
    Jump,
    Coyote,
    AirJump,
    WallJump,
    PlatformJumpDrop
} // end enum JumpType

public interface IPlayerController
{
    public PlayerStats Stats { get; }
    public ControllerState State { get; }
    public event Action<JumpType> Jumped;
    public event Action<bool, float> GroundedChanged;
    public event Action<bool, Vector2> DashChanged;
    public event Action<bool> WallGrabChanged;
    public event Action SizeChanged;
    public event Action<float, bool> UsedDust;
    public event Action<Vector2> Repositioned;
    public event Action<bool, bool> ToggledPlayer;

    public bool Active { get; }
    public Vector2 Up { get; }
    public Vector2 Input { get; }
    public Vector2 GroundNormal { get; }
    public Vector2 Velocity { get; }
    public int WallDirection { get; }
    public bool ClimbingLadder { get; }
    public float CurrentDust { get; }
    public PlayerStates PlayerState { get; set; }

    // External force
    public void AddFrameForce(Vector2 force, bool resetVelocity = false);

    // Utility
    public void LoadState(ControllerState state);
    public void RepositionImmediately(Vector2 position, bool resetVelocity = false);
    public void TogglePlayer(bool on, bool dead = false, bool changeAnimation = true);

    // Dust
    public void ChangeDust(float scalar, bool hostile);

    // Other
    public void Die();
    public bool CanDash();
} // end interface IPlayerController

public enum PlayerStates
{
    Playing,
    Paused,
    Dialogue,
    Dead
} // end enum PlayerStates

public interface ISpeedModifier
{
    public bool InAir { get; }
    public bool OnGround { get; }
    public Vector2 Modifier { get; }
} // end interface ISpeedModifier

// Used to save and load character state
public struct ControllerState
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Velocity;
    public bool Grounded;
} // end struct ControllerState
