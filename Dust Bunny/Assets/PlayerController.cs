using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float moveSmoothing = 0.01f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashTime = 1f;
    [SerializeField] int bunnySize = 1;
    [SerializeField] float bunnySizeScalar = 2f;
    [SerializeField] float scaleSpeed = 0.5f;
    float epsilon = 0.01f;
    Vector3 originalSize = new Vector3(1f,1f,1f);


    Vector2 zeroVelocity = Vector3.zero;
    Vector3 zeroVector3 = Vector3.zero;

    bool canJump =true;
    bool canDash=true;
    bool doDash = false;
    bool isDashing = false;
    bool doJump = false;
    bool growing = false;

    Rigidbody2D thisRigidbody;

    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        mainCamera = FindObjectOfType<Camera>();
        originalSize = transform.localScale;
    }

    float horizontalMovement = 0;
    //Vector2 targetVelocity = Vector2.zero;
    Vector2 dashDirection = Vector2.zero;
    Vector2 jumpForceVector = Vector2.zero;

    void Update()
    {

        horizontalMovement = Input.GetAxisRaw("Horizontal") * moveSpeed;

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            doJump = true;
        }

        // Dash
        if (Input.GetButtonDown("Fire1"))
        {
            doDash = true;

        }

        


        //thisRigidbody.velocity = targetVelocity;

    }

    void FixedUpdate()
    {
        Move(horizontalMovement);

        //     thisRigidbody.velocity = Vector2.SmoothDamp(thisRigidbody.velocity, targetVelocity, ref zeroVelocity, moveSmoothing);
        if (doJump && canJump)
        {
            doJump = false;
            Jump();
        }
        if (doDash && canDash)
        {
            doDash = false;
            StartCoroutine(Dash());
        }

    }

    void Move(float horizontalMovement)
    {
        Vector2 targetVelocity = new Vector2(horizontalMovement * Time.fixedDeltaTime, thisRigidbody.velocity.y);
        // damp movement, smooth damp
        if (isDashing)
        {
            //thisRigidbody.velocity = Vector2.SmoothDamp(thisRigidbody.velocity, new Vector2(thisRigidbody.velocity.x + horizontalMovement, thisRigidbody.velocity.y), ref zeroVelocity, moveSmoothing);
            Vector2 newTargetVelocity = new Vector2(thisRigidbody.velocity.x + horizontalMovement * Time.fixedDeltaTime, thisRigidbody.velocity.y);
            thisRigidbody.velocity = Vector2.SmoothDamp(thisRigidbody.velocity, newTargetVelocity, ref zeroVelocity, moveSmoothing);
        }
        else
        {
            thisRigidbody.velocity = Vector2.SmoothDamp(thisRigidbody.velocity, targetVelocity, ref zeroVelocity, moveSmoothing);
            //thisRigidbody.velocity = targetVelocity;
        }
    }

    void Jump()
    {
        jumpForceVector = new Vector2(0f, jumpForce);
        thisRigidbody.AddForce(jumpForceVector, ForceMode2D.Impulse);
    }



    // Dashes for "dashTime" seconds constantly. Uses AddForce.
    IEnumerator Dash()
    {
        isDashing = true;

        Vector2 mousePos = Input.mousePosition;//mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        dashDirection = (mousePos - playerPos).normalized;

        float gravity = thisRigidbody.gravityScale;
        thisRigidbody.gravityScale = 0f;

        thisRigidbody.AddForce(new Vector2(dashDirection.x * dashForce, dashDirection.y * dashForce), ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashTime);
        thisRigidbody.gravityScale = gravity;
        
        isDashing = false;
    }

    public void ChangeSize(int newSize)
    {
        if (growing == true)
            return;
        Vector3 targetScale = originalSize * Mathf.Pow(bunnySizeScalar, newSize - 1);
        bunnySize = newSize;

        StartCoroutine(GrowDamp(targetScale));

    }

    IEnumerator GrowDamp(Vector3 targetScale)
    {
        growing = true;
        while(Vector3.Distance(transform.localScale, targetScale) > epsilon)
        {
            Vector3 interValue = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
            transform.localScale = interValue;
            yield return null;

        }
        growing = false;

    }




}
