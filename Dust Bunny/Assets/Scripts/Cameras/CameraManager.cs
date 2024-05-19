using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;
    private float _currentCameraOrthographicSize = 8f;
    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }
    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;
    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;
    private float _normYPanAmount;
    private Vector2 _startingTrackedObjectOffset;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        _currentCamera.m_Lens.OrthographicSize = _currentCameraOrthographicSize;
    } // end Awake

    private void Update()
    {
        UpdateShake();
    } // end UpdateShake

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
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }
    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos;
        //set the direction and distance if we are panning in the direction indicated by the trigger object
        if (!panToStartingPos)
        {
            //set the direction and distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }
            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }
        // Handle the direction settings when moving back to the starting position
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }
        // Handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, elapsedTime / panTime);
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

    public void SetOrthographicSize(float newSize, bool tween, float lerpTime = 1.7f, bool addValue = false)
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
    } // end SetCurrentCameraOrthographicSize

    public void SetOrthographicSizeDialogue(float newSize)
    {
        bool tween = true;
        float lerpTime = 1.7f;
        bool addValue = false;
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
    } // end SetCurrentCameraOrthographicSize

    #region Shake
    [SerializeField] private float _defaultShakeIntensity = 1f;
    [SerializeField] private float _defaultshakeTime = 0.2f;
    private CinemachineBasicMultiChannelPerlin _perlin;

    private float _shakeTimer = 0;
    public void ShakeCamera(float shakeIntensity = -1, float shakeTime = -1, CinemachineBasicMultiChannelPerlin perlin = null)
    {
        if (_shakeTimer != 0)
        {
            Debug.Log("ShakeTimer is not 0, another shake is already in progress, overriding the current shake.");
            StopShake();
        }

        if (shakeIntensity == -1) shakeIntensity = _defaultShakeIntensity;
        if (shakeTime == -1) shakeTime = _defaultshakeTime;
        if (perlin == null) _perlin = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = shakeIntensity;
        _shakeTimer = shakeTime;
    } // end ShakeCamera

    public void ShakeCameraDialogue(float shakeIntensity = -1)
    {
        float shakeTime = -1;
        CinemachineBasicMultiChannelPerlin perlin = null;
        if (_shakeTimer != 0)
        {
            Debug.Log("ShakeTimer is not 0, another shake is already in progress, overriding the current shake.");
            StopShake();
        }

        if (shakeIntensity == -1) shakeIntensity = _defaultShakeIntensity;
        if (shakeTime == -1) shakeTime = _defaultshakeTime;
        if (perlin == null) _perlin = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = shakeIntensity;
        _shakeTimer = shakeTime;
    } // end ShakeCameraDialogue

    public void StopShake(CinemachineBasicMultiChannelPerlin perlin = null)
    {
        if (perlin == null) _perlin = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _perlin.m_AmplitudeGain = 0;
        _shakeTimer = 0;
    } // end StopShake

    private void UpdateShake()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0)
            {
                StopShake();
            }
        }
    } // end UpdateShake
    #endregion
} // end class CameraManager
