using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using SpringCleaning.Player;
using UnityEngine;

namespace SpringCleaning.Camera
{
    [HelpURL("https://www.youtube.com/watch?v=9dzBrLUIF8g")]
    public class CameraContolTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool _swapCamerasOnExit = false;
        [ShowIfGroup("_swapCamerasOnExit")]
        [SerializeField, BoxGroup("_swapCamerasOnExit/Cameras")]
        public CinemachineVirtualCamera _cameraOnLeft;
        [SerializeField, BoxGroup("_swapCamerasOnExit/Cameras")]
        public CinemachineVirtualCamera _cameraOnRight;

        [SerializeField]
        private bool _panCameraOnContact = false;
        [ShowIfGroup("_panCameraOnContact")]
        [SerializeField, BoxGroup("_panCameraOnContact/PanSettings")]
        private PanDirection _panDirection;
        [SerializeField, BoxGroup("_panCameraOnContact/PanSettings")]
        private float _panDistance = 3f;
        [SerializeField, BoxGroup("_panCameraOnContact/PanSettings")]
        private float _panTime = 0.35f;

        private Collider2D _col;
        void Awake()
        {
            _col = GetComponent<Collider2D>();
        } // end Awake

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IPlayerController controller)) return;

            if (_panCameraOnContact)
            {
                CameraManager.Instance.PanCameraInDirection(_panDistance, _panDirection, _panTime);
            }

        } // end OnTriggerEnter2D

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IPlayerController controller)) return;

            Vector2 exitDirection = (other.transform.position - _col.bounds.center).normalized;
            if (_swapCamerasOnExit && _cameraOnLeft != null && _cameraOnRight != null)
            {
                CameraManager.Instance.SwapCamera(_cameraOnLeft, _cameraOnRight, exitDirection);
            }

            if (_panCameraOnContact)
            {
                CameraManager.Instance.PanCameraToDefault(_panTime);
            }

        } // end OnTriggerExit2D
    } // end class CameraContolTrigger

    public enum PanDirection
    {
        Up,
        Down,
        Left,
        Right
    } // end enum PanDirecrtion
}
