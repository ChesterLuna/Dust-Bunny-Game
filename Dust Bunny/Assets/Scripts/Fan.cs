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
    SpriteRenderer _fanSprite;
    Collider2D _fanCollider;

    void Awake()
    {
        _fanCollider = GetComponent<Collider2D>();
        _fanSprite = GetComponent<SpriteRenderer>();
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
    }

    public void Enable()
    {
        _fanCollider.enabled = true;
        _fanSprite.enabled = true;
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
            Rigidbody2D player = collider.gameObject.GetComponent<PlayerController>().GetRigidbody2D();
            player.AddForce(transform.up * _force);
        }
    }
}


