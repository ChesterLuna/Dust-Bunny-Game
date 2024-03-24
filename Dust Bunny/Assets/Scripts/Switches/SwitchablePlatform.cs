using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchablePlatform : MonoBehaviour, ISwitchable
{
    [SerializeField] bool _isTimed = false;
    [SerializeField] float _timedToggleLength = 1f;
    SpriteRenderer _sprite;
    Collider2D _collider;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
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
        _collider.enabled = false;
        _sprite.enabled = false;
    } // end Disable

    public void Enable()
    {
        _collider.enabled = true;
        _sprite.enabled = true;
    } // end Enable

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
    } // end Toggle
} // end SwitchablePlatform

