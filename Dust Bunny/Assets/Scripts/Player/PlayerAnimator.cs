using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;


public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [SerializeField] private GameObject _effectsParent;
    [SerializeField] private SpriteRenderer _sprite;


    [Header("Particles")][SerializeField] private ParticleSystem _jumpParticles;
    [SerializeField] private ParticleSystem _launchParticles;
    [SerializeField] private ParticleSystem _moveParticles;
    [SerializeField] private ParticleSystem _landParticles;
    [SerializeField] private ParticleSystem _doubleJumpParticles;
    [SerializeField] private ParticleSystem _useDustParticles;
    [SerializeField] private ParticleSystem _gainDustParticles;
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private ParticleSystem _dashRingParticles;
    [SerializeField] private ParticleSystem _idleParticles;
    [SerializeField] private Transform _dashRingTransform;

    [SerializeField] private Color _particleCanDashColor;
    [SerializeField] private Color _particleCannotDashColor;

    [Header("Audio")]
    [SerializeField] private PlayerSFXController _sfx;


    private IPlayerController _player;
    private GeneratedCharacterSize _character;
    private GameObject _arrowParent;
    private GameObject _arrowPivot;
    private GameObject _arrowBGSprite;
    private GameObject _arrowCirclesParent;
    private GameObject _arrowLocationsParent;
    private SpriteRenderer[] _arrowVisuals;
    private float _arrowVisibility = 0.0f;


    // Animation Variables
    private bool _dashing;
    private bool _jumping;
    private bool _dead;
    private float _playDustGainedParticlesTimer;


    private void Awake()
    {
        _player = GetComponentInParent<IPlayerController>();
        _character = _player.Stats.CharacterSize.GenerateCharacterSize();
        OnSizeChanged(false);

        _arrowParent = transform.GetChild(0).Find("Arrow").gameObject;
        _arrowPivot = _arrowParent.transform.Find("Pivot Point").gameObject;
        _arrowBGSprite = _arrowPivot.transform.Find("Sprite").gameObject;
        _arrowCirclesParent = _arrowParent.transform.Find("Circles").gameObject;
        _arrowLocationsParent = _arrowPivot.transform.Find("Circle locations").gameObject;
        _arrowVisuals = _arrowParent.GetComponentsInChildren<SpriteRenderer>();

        // Fix for dying during slow mo effect
        Time.timeScale = 1.0f;
    } // end Awake

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
        _player.DashChanged += OnDashChanged;
        _player.WallGrabChanged += OnWallGrabChanged;
        _player.SizeChanged += OnSizeChanged;
        _player.ToggledPlayer += PlayerOnToggledPlayer;
        _player.UsedDust += OnUsedDust;
        _player.GainedDust += OnGainedDust;

        _moveParticles.Play();
    } // end OnEnable

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;
        _player.DashChanged -= OnDashChanged;
        _player.WallGrabChanged -= OnWallGrabChanged;
        _player.SizeChanged -= OnSizeChanged;
        _player.ToggledPlayer -= PlayerOnToggledPlayer;
        _player.UsedDust -= OnUsedDust;
        _player.GainedDust -= OnGainedDust;

        _moveParticles.Stop();
    } // end OnDisable

    private void Update()
    {
        if (_player == null) return;

        var xInput = _player.Input.x;

        // SetParticleColor(-_player.Up, _moveParticles);

        HandleSpriteFlip(xInput);

        HandleWallSlideEffects();

        HandleAnimations();

        //Handle idle particle color
        ParticleSystem.MainModule _idleParticlesMain = _idleParticles.main;
        if (_player.CanDash())
        {
            _idleParticlesMain.startColor = _particleCanDashColor;
        }
        else
        {
            _idleParticlesMain.startColor = _particleCannotDashColor;
        }

        //Handle on dust gain particle stopping
        if (_playDustGainedParticlesTimer < 0.0f)
        {
            _playDustGainedParticlesTimer = 0.0f;
            _gainDustParticles.Stop();
        }
        else
        {
            _playDustGainedParticlesTimer -= Time.deltaTime;
        }

        HandleDashArrow();


    } // end Update

    private void HandleDashArrow(){
        float cameraZoom = Camera.main.orthographicSize;

        //Get the dash target from mouse position or keyboard input, based on UseMouseForDash
        Vector3 dashTargetWorldPosition;
        if(UserInput.instance.UseMouseForDash) {
            dashTargetWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashTargetWorldPosition.z = 0;
        } else {
            Vector2 moveInput = dashTargetWorldPosition = UserInput.instance.Gather().Move;
            float dashLengthMod = 1 + (.8f * _player.Stats.DashDuration * _player.Stats.DashVelocity * transform.localScale.x);
            if(moveInput.sqrMagnitude > 0.001f){
                dashTargetWorldPosition = _arrowPivot.transform.position + (Vector3)(moveInput * dashLengthMod);
            } else {
                float mod = 1;
                if(_sprite.flipX) mod = -1;
                dashTargetWorldPosition = _arrowPivot.transform.position + new Vector3(mod * dashLengthMod, 0, 0);
            }
        }

        // deep math i do not understand
        Vector3 perpendicular = Vector3.Cross(_arrowPivot.transform.position - dashTargetWorldPosition, Vector3.forward);
		_arrowPivot.transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);

        // move the circles to the correct location
        for(int i = 0; i < _arrowCirclesParent.transform.childCount; i++){
            _arrowCirclesParent.transform.GetChild(i).position = _arrowLocationsParent.transform.GetChild(i).position;
        }

        // handle arrow length
        if(transform.localScale.x > 0 && transform.localScale.y > 0){
            _arrowParent.transform.localScale = new Vector3(1/transform.localScale.x, 1/transform.localScale.y, 1); // we do NOT want the arrow to respect player size: we want to handle that ourselves based on mouse pos instead
        }
        float distance = Vector3.Distance(dashTargetWorldPosition, transform.position);
        float length = Mathf.Max(Mathf.Min(distance / (4.0f + 0.1f * cameraZoom), 2 * cameraZoom), 0.05f * cameraZoom);
        _arrowPivot.transform.localScale = new Vector3(length, _arrowPivot.transform.localScale.y, _arrowPivot.transform.localScale.z);

        // handle opacity
        if(UserInput.instance.Gather().DashHeld){
            _arrowVisibility = Mathf.Lerp(_arrowVisibility, 0.8f, Time.deltaTime * 3);
        } else {
            _arrowVisibility = Mathf.Lerp(_arrowVisibility, 0f, Time.deltaTime * 10);
        }

        // set opacity of all graphics
        for(int i = 0; i < _arrowVisuals.Length; i++){
            _arrowVisuals[i].color = new Color( _arrowVisuals[i].color.r,  _arrowVisuals[i].color.g,  _arrowVisuals[i].color.b, _arrowVisibility);
        }
        _arrowBGSprite.GetComponent<SpriteRenderer>().material.SetFloat("Visibility", _arrowVisibility);
    }

    #region Walls & Ladders

    [Header("Walls & Ladders")]
    [SerializeField]
    private ParticleSystem _wallSlideParticles;

    [SerializeField] private float _wallSlideParticleOffset = 0.3f;
    [SerializeField] private float _distancePerClimbSound = 0.2f;

    private bool _isOnWall, _isSliding;
    private bool _ascendingLadder;
    private float _lastClimbSoundY;

    private void OnWallGrabChanged(bool onWall)
    {
        _isOnWall = onWall;
        if (_isOnWall) _sfx.PlaySFX(PlayerSFXController.SFX.Foot_Step); // TODO: Wall Grab SFX
    } // end OnWallGrabChanged

    private void HandleWallSlideEffects()
    {
        var slidingThisFrame = _isOnWall && !_grounded && _player.Velocity.y < 0;

        if (!_isSliding && slidingThisFrame)
        {
            _isSliding = true;
            _wallSlideParticles.Play();
        }
        else if (_isSliding && !slidingThisFrame)
        {
            _isSliding = false;
            _wallSlideParticles.Stop();
        }

        // SetParticleColor(new Vector2(_player.WallDirection, 0), _wallSlideParticles);
        _wallSlideParticles.transform.localPosition = new Vector3(_wallSlideParticleOffset * _player.WallDirection, 0, 0);

        if ((_player.ClimbingLadder || _isOnWall) && _player.Velocity.y > 0)
        {
            if (!_ascendingLadder)
            {
                _ascendingLadder = true;
                _lastClimbSoundY = transform.position.y;
                Play();
            }

            if (transform.position.y >= _lastClimbSoundY + _distancePerClimbSound)
            {
                _lastClimbSoundY = transform.position.y;
                Play();
            }
        }
        else
        {
            _ascendingLadder = false;
        }

        void Play()
        {
            if (_isOnWall) _sfx.PlayWallClimbSound();
            else _sfx.PlayLadderClimbSound();
        }
    } // end HandleWallSlideEffects

    #endregion

    #region Animation
    private void HandleSpriteFlip(float xInput)
    {
        if (_player.Input.x != 0) _sprite.flipX = xInput < 0;
    } // end HandleSpriteFlip

    #endregion

    #region Event Callbacks

    private void OnJumped(JumpType type)
    {
        if (type is JumpType.Jump or JumpType.Coyote or JumpType.WallJump)
        {
            _jumping = true;
            _sfx.PlaySFX(PlayerSFXController.SFX.Jump);

            // Only play particles when grounded (avoid coyote)
            if (type is JumpType.Jump)
            {
                // SetColor(_jumpParticles);
                // SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }
        else if (type is JumpType.AirJump)
        {
            _sfx.PlaySFX(PlayerSFXController.SFX.Jump); // TODO: Double Jump SFX
            _doubleJumpParticles.Play();
        }
    } // end OnJumped

    private bool _grounded;

    private void OnGroundedChanged(bool grounded, float impact)
    {
        _grounded = grounded;

        if (grounded)
        {
            _sfx.PlaySFX(PlayerSFXController.SFX.Land);
            _moveParticles.Play();

            _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
            // SetColor(_landParticles);
            _landParticles.Play();
        }
        else
        {
            _moveParticles.Stop();
        }
    } // end OnGroundedChanged


    private void OnDashChanged(bool dashing, Vector2 dir)
    {
        if (dashing)
        {
            _dashParticles.Play();
            _dashRingTransform.up = dir;
            _dashRingParticles.Play();
            _sfx.PlaySFX(PlayerSFXController.SFX.Dash);
        }
        else
        {
            _dashParticles.Stop();
        }
    } // end OnDashChanged

    [SerializeField] Vector2 _sizeFactor = new Vector2(0.69f, 0.61f);
    [SerializeField] private float lerpTime = 1.7f;
    private void OnSizeChanged(bool tween = true)
    {
        _character = _player.Stats.CharacterSize.GenerateCharacterSize();
        Vector2 newSize = new Vector2(_character.Width / _sizeFactor.x, _character.Height / _sizeFactor.y);
        float actualLerpTime = tween ? lerpTime : 0.01f;
        gameObject.transform?.DOScale(newSize, actualLerpTime);
    } // end OnSizeChanged


    private void OnDead()
    {
        _dead = true;
        _sfx.PlaySFX(PlayerSFXController.SFX.Dead);
    } // end OnDead

    private void OnUsedDust(float dustAmount, bool hostile)
    {
        if (hostile)
        {
            _sfx.PlaySFX(PlayerSFXController.SFX.Took_Damage);
            StartCoroutine(FreezeGameOnTakeDamage());
        }
        _useDustParticles.Play();
    } // end OnUsedDust

    private void OnGainedDust(float gainedAmount)
    {

        _playDustGainedParticlesTimer += Mathf.Min(gainedAmount / 20.0f, 3.0f);
        if (_gainDustParticles.isStopped)
        {
            _gainDustParticles.Play();
        }
        _sfx.PlaySFX(PlayerSFXController.SFX.Dust_Collect_Full);
    }

    private IEnumerator FreezeGameOnTakeDamage()
    {
        // Slow down the game effect, maybe undesirable
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.023f);
        Time.timeScale = 1;

        yield return null;
    }
    #endregion

    private void PlayerOnToggledPlayer(bool on, bool dead)
    {
        if (dead)
        {
            OnDead();
        }
        else
        {
            _effectsParent.SetActive(on);
        }
    } // end PlayerOnToggledPlayer

    #region Helpers

    private ParticleSystem.MinMaxGradient _currentGradient;

    private void SetParticleColor(Vector2 detectionDir, ParticleSystem system)
    {
        var ray = Physics2D.Raycast(transform.position, detectionDir, 2);
        if (!ray) return;

        ray.transform.TryGetComponent(out SpriteRenderer r); // Note this does not work on tilemaps
        if (r)
        {
            // _currentGradient = new ParticleSystem.MinMaxGradient(r.color * 0.9f, r.color * 1.2f); // Old method, using color set by sprite

            // New method, using average color of sprite
            Color averageColor = GetAverageColor(r.sprite.texture.GetPixels());
            _currentGradient = new ParticleSystem.MinMaxGradient(averageColor * 0.9f, averageColor * 1.2f);
        }
        else
        {
            // Default to white
            _currentGradient = new ParticleSystem.MinMaxGradient(Color.white);
        }
        SetColor(system);
    } // end SetParticleColor

    private Color GetAverageColor(Color[] colors)
    {
        float r = 0;
        float g = 0;
        float b = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            r += colors[i].r;
            g += colors[i].g;
            b += colors[i].b;
        }
        return new Color(r / colors.Length, g / colors.Length, b / colors.Length);
    } // end GetAverageColor


    private void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = _currentGradient;
    } // end SetColor

    public void OnPlayerFootstep()
    {
        _sfx.PlaySFX(PlayerSFXController.SFX.Foot_Step);
    } // end OnPlayerFootstep

    #endregion

    #region Animation
    private float _lockedTill;
    private void HandleAnimations()
    {
        if (!_anim.isActiveAndEnabled || _anim == null) return;
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
            else if (_player.PlayerState == PlayerStates.Dialogue)
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
                if (_player.Input.x == 0f)
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
                if (_player.Velocity.y > 0)
                {
                    return Rising;
                }
                else if (_player.Velocity.y < 0)
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
