using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : MonoBehaviour, ISwitchable
{
    [SerializeField] bool _isTimed = false;
    [SerializeField] float _timedToggleLength = 1f;
    [SerializeField] float _force = 75;
    [SerializeField] SpriteRenderer _baseSprite;
    [SerializeField] SpriteRenderer _fanSprite;
    Collider2D _fanCollider;
    Animator _animator;

    void Awake()
    {
        _fanCollider = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
    } // end Awake

    void Start()
    {
        if (_isTimed)
        {
            InvokeRepeating("Toggle", 0f, _timedToggleLength);
        }
    } // end Start

    public void Disable()
    {
        _fanCollider.enabled = false;
        _fanSprite.enabled = false;
        _animator.speed = 0;
    } // end Disable

    public void Enable()
    {
        _fanCollider.enabled = true;
        _fanSprite.enabled = true;
        _animator.speed = 1;
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
    } // end OnTriggerStay2D
} // end class Fan