using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roomba : MonoBehaviour
{
    // [SerializeField] Transform _raycastOrigin;
    // [SerializeField] float _moveSpeed = 2f;
    // [SerializeField] LayerMask _environmentLayer;
    // [SerializeField] float _gravity = 9.8f;

    // private Rigidbody2D _rb;

    // void Awake()
    // {
    //     _rb = GetComponent<Rigidbody2D>();
    //     _rb.isKinematic = true;
    // }
    // private void FixedUpdate()
    // {
    //     if (checkForFlip())
    //     {
    //         Vector3 newScale = transform.localScale;
    //         newScale.y *= -1;
    //         transform.localScale = newScale;
    //         transform.Rotate(0, 180, 0);
    //     }

    //     Vector3 fallVector = Vector3.zero;
    //     if (!grounded())
    //     {
    //         fallVector = -transform.up * _gravity * Time.fixedDeltaTime;
    //     }
    //     Vector3 moveVector = transform.right * _moveSpeed * Time.fixedDeltaTime;
    //     _rb.MovePosition(transform.position + moveVector + fallVector);
    // } // end FixedUpdate


    // private bool grounded()
    // {
    //     Vector2 raycastOrigin = transform.position;
    //     float downRaycastDistance = 0.51f;

    //     RaycastHit2D downHit = Physics2D.Raycast(raycastOrigin, -(Vector2)transform.up, downRaycastDistance, _environmentLayer);
    //     Debug.DrawRay(raycastOrigin, -(Vector2)transform.up * downRaycastDistance, Color.red);
    //     return downHit.collider != null;
    // }


    // private bool checkForFlip()
    // {
    //     Vector2 raycastOrigin = _raycastOrigin.position;
    //     float rightRaycastDistance = 0.01f;
    //     float downRaycastDistance = 0.51f;

    //     RaycastHit2D downHit = Physics2D.Raycast(raycastOrigin, -(Vector2)transform.up, downRaycastDistance, _environmentLayer);
    //     RaycastHit2D rightHit = Physics2D.Raycast(raycastOrigin, (Vector2)transform.right, rightRaycastDistance, _environmentLayer);
    //     Debug.DrawRay(raycastOrigin, -(Vector2)transform.up * downRaycastDistance, Color.red);
    //     Debug.DrawRay(raycastOrigin, (Vector2)transform.right * rightRaycastDistance, Color.red);


    //     return (downHit.collider == null && grounded()) || rightHit.collider != null;
    // }
}
