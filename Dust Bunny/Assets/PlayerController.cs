using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    bool canJump;
    bool canDash;
    Rigidbody2D thisRigidbody;
    PlayerSFXController sfx;

    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        sfx = gameObject.GetComponentInChildren<PlayerSFXController>();
    }



    void Update()
    {

        float horizontalMovement = Input.GetAxisRaw("Horizontal") * moveSpeed; // time delta time? not sure

        thisRigidbody.velocity = new Vector2(horizontalMovement, thisRigidbody.velocity.y);

        // damp movement, smooth damp



        if (Input.GetButtonDown("Jump"))
        {
            // Jump
            Vector2 jumpForceVector = new Vector2(0, jumpForce);
            thisRigidbody.AddForce(jumpForceVector, ForceMode2D.Impulse);
            sfx.PlaySFX(PlayerSFXController.SFX.Jump);
        }
    }
    




}
