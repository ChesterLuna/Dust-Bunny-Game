using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchablePlatform : Switchable
{
    [SerializeField] bool _isTimed = false;
    [SerializeField] float _timedToggleLength = 1f;
    SpriteRenderer _sprite;
    Collider2D _collider;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (_isTimed)
        {
            InvokeRepeating("Toggle", 0f, _timedToggleLength);
        }
    }

    public override void Disable()
    {
        _collider.enabled = false;
        _sprite.enabled = false;
    }

    public override void Enable()
    {
        _collider.enabled = true;
        _sprite.enabled = true;
    }

    public void Toggle()
    {
        if (_collider.enabled && _sprite.enabled)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    }
}
