using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SpringCleaning.Player;
namespace SpringCleaning.Camera
{
    /// <summary>
    /// None of this is used as of now.
    /// </summary>
    [HelpURL("https://www.youtube.com/watch?v=9dzBrLUIF8g")]
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
            // _player = _playerTransform.gameObject.GetComponent<PlayerController>() != null ? _playerTransform.gameObject.GetComponent<PlayerController>() : _playerTransform.parent.gameObject.GetComponent<PlayerController>();
            _isFacingRight = _player.Right == new Vector2(1, 0) ? true : false;
        } // end Awake

        private void Update()
        {
            transform.position = _playerTransform.position;
        } // end Update

        public void CallTurn()
        {
            transform.DORotate(new Vector3(0, DetermineEndRotation(), 0), _flipYRotationTime).SetEase(Ease.InOutSine);
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
}