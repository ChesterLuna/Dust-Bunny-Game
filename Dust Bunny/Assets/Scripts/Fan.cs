using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : Switchable
{
    [SerializeField] float _force = 75;
    [SerializeField] SpriteRenderer _baseSprite;
    SpriteRenderer _fanSprite;
    Collider2D _fanCollider;

    void Awake()
    {
        _fanCollider = GetComponent<Collider2D>();
        _fanSprite = GetComponent<SpriteRenderer>();
    }

    public override void disable()
    {
        _fanCollider.enabled = false;
        _fanSprite.enabled = false;
    }

    public override void enable()
    {
        _fanCollider.enabled = true;
        _fanSprite.enabled = true;
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


