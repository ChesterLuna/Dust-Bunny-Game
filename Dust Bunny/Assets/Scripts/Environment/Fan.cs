using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : MonoBehaviour, ISwitchable
{
    [Tooltip("The time between toggles (on/off), set to 0 to disable")]
    [SerializeField] float _timedToggleLength = 1f;
    [SerializeField] float _force = 75;
    [SerializeField] SpriteRenderer _baseSprite;
    [SerializeField] SpriteRenderer _fanSprite;
    Collider2D _fanCollider;
    Animator _animator;
    ParticleSystem _particles;

    void Awake()
    {
        _fanCollider = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
        _particles = GetComponentInChildren<ParticleSystem>();
        if (_particles != null){
            _particles.transform.localScale = new Vector3(_particles.transform.localScale.x, _particles.transform.localScale.y, _particles.transform.localScale.z * transform.localScale.y);
            _particles.Play();
        }
    } // end Awake

    void Start()
    {
        if (_timedToggleLength > 0)
        {
            InvokeRepeating("Toggle", 0f, _timedToggleLength);
        }
    } // end Start

    public void Disable()
    {
        _fanCollider.enabled = false;
        _fanSprite.enabled = false;
        _animator.speed = 0;
        _particles.Stop();
    } // end Disable

    public void Enable()
    {
        _fanCollider.enabled = true;
        _fanSprite.enabled = true;
        _animator.speed = 1;
        _particles.Play();
    } // end Enable

    public void Toggle()
    {
        if (_fanCollider.enabled && _fanSprite.enabled)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    } // end Toggle

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;
        Vector2 force = transform.up * _force;
        controller.AddFrameForce(force, true);
        controller.ResetAirJumps();
        controller.ResetDashes();
    } // end OnTriggerStay2D
} // end class Fan