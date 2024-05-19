using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace SpringCleaning.Camera
{
    [HelpURL("https://www.youtube.com/watch?v=9dzBrLUIF8g")]
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;
        [SerializeField, ValidateInput(nameof(ValidateVirtualCameras))] private CinemachineVirtualCamera[] _allVirtualCameras;

        [Title("Controls for Screen Shake")]
        [SerializeField] private float _defaultShakeForce = 1f;

        [Title("Controls for lerping the Y Damping during player jump/fall")]
        [SerializeField] private float _fallPanAmount = 0.25f;
        [SerializeField] private float _fallYPanTime = 0.35f;
        public float _fallSpeedYDampingChangeThreshold = -15f;
        public bool IsLerpingYDamping { get; private set; }
        public bool LerpedFromPlayerFalling { get; set; }
        private Coroutine _lerpYPanCoroutine;
        private Coroutine _panCameraCoroutine;
        private float _normYPanAmount;

        private CinemachineVirtualCamera _currentCamera;
        private CinemachineFramingTransposer _framingTransposer;
        private Vector2 _startingTrackedObjectOffset;
        private float _currentCameraOrthographicSize = 8f;

        private bool ValidateVirtualCameras(CinemachineVirtualCamera[] virtualCameras, ref string errorMessage)
        {
            if (virtualCameras.Length == 0)
            {
                errorMessage = "There are no virtual cameras assigned to the CameraManager";
                return false;
            }

            List<int> enabledVirtualCameras = new List<int>();
            for (int i = 0; i < _allVirtualCameras.Length; i++)
            {
                if (_allVirtualCameras[i].GetComponent<CinemachineImpulseListener>() == null)
                {
                    Debug.LogWarning("There was no CinemachineImpulseListener component attached to the virtual camera at index " + i + " in the CameraManager. One has been added for you.");
                    _allVirtualCameras[i].AddComponent<CinemachineImpulseListener>();
                }

                if (_allVirtualCameras[i].enabled)
                {
                    enabledVirtualCameras.Add(i);
                }
            }
            if (enabledVirtualCameras.Count == 0)
            {
                errorMessage = "There needs to be one virtual camera enabled in the CameraManager";
                return false;
            }
            else if (enabledVirtualCameras.Count > 1)
            {
                errorMessage = "There can only be one virtual camera enabled in the CameraManager. Currently indexs: ";
                for (int i = 0; i < enabledVirtualCameras.Count; i++)
                {
                    if (i == enabledVirtualCameras.Count - 1)
                    {
                        errorMessage += "and " + enabledVirtualCameras[i] + " are enabled";
                    }
                    else
                    {
                        errorMessage += enabledVirtualCameras[i] + ", ";
                    }
                }
                return false;
            }

            return true;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            for (int i = 0; i < _allVirtualCameras.Length; i++)
            {
                if (_allVirtualCameras[i].enabled)
                {
                    _currentCamera = _allVirtualCameras[i];
                    _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                }
            }
            // Set the YDamping amount so it's based on the inspector value
            _normYPanAmount = _framingTransposer.m_YDamping;

            // Set the starting position of the tracked object offset
            _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;

            // Set the orthographic size of the camera
            _currentCameraOrthographicSize = _currentCamera.m_Lens.OrthographicSize;
        } // end Awake

        #region Lerp the Y Damping
        public void LerpYDamping(bool isPlayerFalling)
        {
            _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
        } // end LerpYDamping

        private IEnumerator LerpYAction(bool isPlayerFalling)
        {
            IsLerpingYDamping = true;
            // Grab the starting damping amount
            float startDampAmount = _framingTransposer.m_YDamping;
            float endDampAmount;
            // Determine the end damping amount
            if (isPlayerFalling)
            {
                endDampAmount = _fallPanAmount;
                LerpedFromPlayerFalling = true;
            }
            else
            {
                endDampAmount = _normYPanAmount;
            }

            // Lerp the pan amount
            float elapsedTime = 0f;
            while (elapsedTime < _fallYPanTime)
            {
                elapsedTime += Time.deltaTime;

                float LerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / _fallYPanTime);
                _framingTransposer.m_YDamping = LerpedPanAmount;
                yield return null;
            }
            IsLerpingYDamping = false;
        } // end LerpYAction

        #endregion

        #region Pan Camera
        public void PanCameraInDirection(float panDistance, PanDirection panDirection, float panTime)
        {
            Vector2 startingPos = _framingTransposer.m_TrackedObjectOffset;
            Vector2 endingPos = Vector2.zero;

            // Set the direction and distance if we are panning in the direction indicated by the trigger object
            switch (panDirection)
            {
                case PanDirection.Up:
                    endingPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endingPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endingPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endingPos = Vector2.left;
                    break;
                default:
                    break;
            }
            endingPos = (endingPos * panDistance) + startingPos;

            if (_panCameraCoroutine != null) StopCoroutine(_panCameraCoroutine);
            _panCameraCoroutine = StartCoroutine(PanCamera(startingPos, endingPos, panTime));
        }

        public void PanCameraToTarget(Vector2 panTarget, float panTime)
        {
            if (_panCameraCoroutine != null) StopCoroutine(_panCameraCoroutine);
            _panCameraCoroutine = StartCoroutine(PanCamera(_framingTransposer.m_TrackedObjectOffset, panTarget, panTime));
        }

        public void PanCameraToDefault(float panTime)
        {
            if (_panCameraCoroutine != null) StopCoroutine(_panCameraCoroutine);
            _panCameraCoroutine = StartCoroutine(PanCamera(_framingTransposer.m_TrackedObjectOffset, _startingTrackedObjectOffset, panTime));
        }

        private IEnumerator PanCamera(Vector2 startingPos, Vector2 endingPos, float panTime)
        {
            // Handle the actual panning of the camera
            float elapsedTime = 0f;
            while (elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;
                Vector3 panLerp = Vector3.Lerp(startingPos, endingPos, elapsedTime / panTime);
                _framingTransposer.m_TrackedObjectOffset = panLerp;
                yield return null;
            }
        }
        #endregion

        #region Swap Cameras
        public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
        {
            // If the current camera is the camera on the left and our trigger exit direction was on the right
            if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
            {
                // Activate the new camera
                cameraFromRight.enabled = true;
                // Deactivate the old camera
                cameraFromLeft.enabled = false;
                // Set the new camera as the current camera
                _currentCamera = cameraFromRight;
                // Update our composer variable
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            // If the currnet camera is the camera on theright and our trigger hit direction was on the left
            else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
            {
                // Activate the new camera
                cameraFromLeft.enabled = true;
                // Deactivate the old camera
                cameraFromRight.enabled = false;
                // Set the new camera as the current camera
                _currentCamera = cameraFromLeft;
                // Update our composer variable
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            _currentCamera.m_Lens.OrthographicSize = _currentCameraOrthographicSize;
        } // end SwapCamera
        #endregion

        #region Set Camera Orthographic Size
        public void SetOrthographicSize(float newSize, bool tween = true, float lerpTime = 1.7f, bool addValue = false)
        {
            if (_currentCamera == null) return;

            if (addValue) newSize += _currentCamera.m_Lens.OrthographicSize;
            if (newSize < 1)
            {
                newSize = 1;
                Debug.LogWarning("Orthographic size cannot be less than 1");
            }

            float actualLerpTime = tween ? lerpTime : 0.01f;
            DOTween.To(() => _currentCamera.m_Lens.OrthographicSize, x => _currentCamera.m_Lens.OrthographicSize = x, newSize, actualLerpTime);
            _currentCameraOrthographicSize = newSize;
        } // end SetOrthographicSize

        /// <summary>
        /// Function with minimal parameters to set the orthographic size of the camera via animation
        /// </summary>
        /// <param name="newSize"></param>
        public void SetOrthographicSize(float newSize)
        {
            SetOrthographicSize(newSize);
        } // end SetOrthographicSize
        #endregion

        #region Shake Camera
        // Based of off https://www.youtube.com/watch?v=CgyLIWyDXqo&t=479s

        /// <summary>
        /// Simple shake with the default shake force
        /// </summary>
        /// <param name="impulseSource"></param>
        public void ShakeCamera(CinemachineImpulseSource impulseSource)
        {
            impulseSource.GenerateImpulseWithForce(_defaultShakeForce);
        } // end ShakeCamera

        /// <summary>
        /// Simple shake with a custom shake force
        /// </summary>
        /// <param name="impulseSource"></param>
        public void ShakeCamera(CinemachineImpulseSource impulseSource, float force)
        {
            impulseSource.GenerateImpulseWithForce(force);
        } // end ShakeCamera

        /// <summary>
        /// Shake the camera with a custom shake force profile
        /// </summary>
        /// <param name="impulseSource"></param>
        /// <param name="profile"></param>
        public void ShakeCamera(CinemachineImpulseSource impulseSource, ScreenShakeProfileSO profile)
        {
            // Save the previous source settings
            float previousImpulseDuration = impulseSource.m_ImpulseDefinition.m_ImpulseDuration;
            AnimationCurve previousImpulseShape = impulseSource.m_ImpulseDefinition.m_CustomImpulseShape;
            Vector3 previousDefaultVelocity = impulseSource.m_DefaultVelocity;

            // Update the source settings
            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = profile.ImpulseDuration;
            impulseSource.m_ImpulseDefinition.m_CustomImpulseShape = profile.ImpluseCurve;
            impulseSource.m_DefaultVelocity = profile.DefaultVelocity;

            // Get the Listener component
            if (!_currentCamera.TryGetComponent<CinemachineImpulseListener>(out CinemachineImpulseListener impulseListener))
            {
                Debug.LogWarning("There was no CinemachineImpulseListener component attached to the current camera. One has been added for you.");
                impulseListener = _currentCamera.gameObject.AddComponent<CinemachineImpulseListener>();
            }

            // Save the previous listener settings
            CinemachineImpulseListener.ImpulseReaction previousReactionSettings = impulseListener.m_ReactionSettings;

            // Update the listener settings
            impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.ListenerAmplitude;
            impulseListener.m_ReactionSettings.m_FrequencyGain = profile.ListenerFrequency;
            impulseListener.m_ReactionSettings.m_Duration = profile.ListenerDuration;

            // Generate the impulse
            impulseSource.GenerateImpulseWithForce(profile.ImpluseForce);

            // Reset the source settings
            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = previousImpulseDuration;
            impulseSource.m_ImpulseDefinition.m_CustomImpulseShape = previousImpulseShape;
            impulseSource.m_DefaultVelocity = previousDefaultVelocity;

            // Reset the listener settings
            impulseListener.m_ReactionSettings = previousReactionSettings;

        } // end ShakeCamera
        #endregion
    } // end class CameraManager
}