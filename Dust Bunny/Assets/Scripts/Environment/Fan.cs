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
    }

    void Start()
    {
        if (_isTimed)
        {
            InvokeRepeating("Toggle", 0f, _timedToggleLength);
        }
    }
    public void Disable()
    {
        _fanCollider.enabled = false;
        _fanSprite.enabled = false;
        _animator.speed = 0;
    }

    public void Enable()
    {
        _fanCollider.enabled = true;
        _fanSprite.enabled = true;
        _animator.speed = 1;
    }

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
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Rigidbody2D player = collider.gameObject.GetComponent<PlayerController>().RB;
            player.AddForce(transform.up * _force);
        }
    }
}


