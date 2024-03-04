using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerController _player;
    private Animator _anim;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    } // end Awake

    private void Start()
    {
        _player.Jumped += OnPlayerOnJumped;
        _player.DoubleJumped += PlayerOnDoubleJumped;
        _player.GroundedChanged += PlayerOnGroundedChanged;
        _player.DashingChanged += PlayerOnDashingChanged;
        _player.Dead += PlayerOnDead;
    } // end Start

    private void Update()
    {
        HandleAnimations();
    } // end Update


    #region Jumping

    [Header("JUMPING")][SerializeField] private float _minImpactForce = 15;

    private bool _grounded;
    private bool _jumping;


    public void OnPlayerFootstep()
    {
        _player.SFX.PlaySFX(PlayerSFXController.SFX.Foot_Step);
    }

    private void OnPlayerOnJumped()
    {
        _jumping = true;
        _player.SFX.PlaySFX(PlayerSFXController.SFX.Jump);
    } // end OnPlayerOnJumped

    private void PlayerOnDoubleJumped()
    {
        _jumping = true;
        _player.SFX.PlaySFX(PlayerSFXController.SFX.Jump);
    } // end PlayerOnDoubleJumped

    private void PlayerOnGroundedChanged(bool grounded, float impactForce)
    {
        if (grounded && !_grounded)
        {
            float p = Mathf.InverseLerp(0, _minImpactForce, impactForce);
            if (impactForce >= _minImpactForce)
            {
                // Debug.Log("Landed with force: " + impactForce);
                _player.SFX.PlaySFX(PlayerSFXController.SFX.Land);
            }
        }
        _grounded = grounded;
    } // end PlayerOnGroundedChanged

    #endregion

    #region Dash

    private bool _dashing;

    private void PlayerOnDashingChanged(bool dashing, Vector2 dir)
    {
        _dashing = dashing;
        if (_dashing)
        {
            _player.SFX.PlaySFX(PlayerSFXController.SFX.Dash);
        }
    } // end PlayerOnDashingChanged

    #endregion

    #region Dead

    private bool _dead;

    private void PlayerOnDead()
    {
        _dead = true;
        _player.SFX.PlaySFX(PlayerSFXController.SFX.Dead);
    } // end PlayerOnDead

    #endregion


    #region Animation
    private float _lockedTill;
    private void HandleAnimations()
    {
        int state = GetState();
        if (state == _currentState)
        {
            return;
        }
        _anim.CrossFade(state, 0, 0);
        _currentState = state;

        int GetState()
        {
            if (Time.time < _lockedTill)
            {
                return _currentState;
            }
            else if (_player.PlayerState == PlayerController.PlayerStates.Dialogue)
            {
                return Idle;
            }
            else if (_dashing)
            {
                return Dash;
            }
            if (_jumping)
            {
                _jumping = false;
                return LockState(JumpStart, 0.20f);
            }
            else if (_grounded)
            {
                if (UserInput.instance.MoveInput.x == 0f)
                {
                    return Idle;
                }
                else
                {
                    return Run;
                }

            }
            else if (_dead)
            {
                return Death;
            }
            else
            {
                if (_rb.velocity.y > 0)
                {
                    return Rising;
                }
                else if (_rb.velocity.y < 0)
                {
                    return Falling;
                }
                // When all else fails, die.
                return Death;
            }

            int LockState(int state, float t)
            {
                _lockedTill = Time.time + t;
                return state;
            } // end LockState
        } // end GetState
    } // end HandleAnimations

    #region Cached Properties

    // Used For debugging
    public string HashToString(int hash)
    {
        if (hash == Idle)
        {
            return "Idle";
        }
        else if (hash == Run)
        {
            return "Run";
        }
        else if (hash == Dash)
        {
            return "Dash";
        }
        else if (hash == JumpStart)
        {
            return "JumpStart";
        }
        else if (hash == Rising)
        {
            return "Rising";
        }
        else if (hash == Falling)
        {
            return "Falling";
        }
        else if (hash == Death)
        {
            return "Death";
        }
        else
        {
            return "Unknown";
        }
    } // end HashToString

    private int _currentState;
    private static readonly int Idle = Animator.StringToHash("Player_Stationary");
    private static readonly int Run = Animator.StringToHash("Player_Walk");
    private static readonly int Dash = Animator.StringToHash("Player_Dash");
    private static readonly int JumpStart = Animator.StringToHash("Player_Jump_Start");
    private static readonly int Rising = Animator.StringToHash("Player_Jump_Rise");
    private static readonly int Falling = Animator.StringToHash("Player_Jump_Fall");
    private static readonly int Death = Animator.StringToHash("Player_Death");

    #endregion

    #endregion
} // end class PlayerAnimator
