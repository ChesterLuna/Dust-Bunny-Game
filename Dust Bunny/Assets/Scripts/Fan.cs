using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : Switchable
{
    [SerializeField] float force = 75;
    [SerializeField] SpriteRenderer baseSprite;
    SpriteRenderer fanSprite;
    Collider2D fanCollider;

    void Awake()
    {
        fanCollider = GetComponent<Collider2D>();
        fanSprite = GetComponent<SpriteRenderer>();
    }

    public override void disable()
    {
        fanCollider.enabled = false;
        fanSprite.enabled = false;
    }

    public override void enable()
    {
        fanCollider.enabled = true;
        fanSprite.enabled = true;
    }


    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Rigidbody2D player = collider.gameObject.GetComponent<PlayerController>().GetRigidbody2D();
            player.AddForce(transform.up * force);
        }
    }
}


