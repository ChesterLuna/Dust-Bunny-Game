using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;
    private PlayerController _player;
    private bool _isFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<PlayerController>();
        _isFacingRight = _player.Right == new Vector2(1, 0) ? true : false;
    } // end Awake

    private void Update()
    {
        transform.position = _playerTransform.position;
    } // end Update

    public void CallTurn()
    {
        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
    } // end CallTurn

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    } // end DetermineEndRotation
} // end class CameraFollowObject
