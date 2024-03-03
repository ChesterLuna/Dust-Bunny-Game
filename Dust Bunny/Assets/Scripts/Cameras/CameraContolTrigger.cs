using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class CameraContolTrigger : MonoBehaviour
{

    public CustomInspectorObjects customInspectorObjects;
    private Collider2D _col;
    void Awake()
    {
        _col = GetComponent<Collider2D>();
    } // end Awake

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (customInspectorObjects.PanCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    } // end OnTriggerEnter2D

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 exitDirection = (other.transform.position - _col.bounds.center).normalized;
            if (customInspectorObjects.SwapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if (customInspectorObjects.PanCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    } // end OnTriggerExit2D
} // end class CameraContolTrigger

[System.Serializable]
public class CustomInspectorObjects
{
    public bool SwapCameras = false;
    public bool PanCameraOnContact = false;
    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;
    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
} // end class CustomInspectorObjects

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
} // end enum PanDirecrtion

#if UNITY_EDITOR
[CustomEditor(typeof(CameraContolTrigger))]
public class MyScriptEditor : Editor
{
    CameraContolTrigger CameraContolTrigger;
    private void OnEnable()
    {
        CameraContolTrigger = (CameraContolTrigger)target;
    } // end OnEnable

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (CameraContolTrigger.customInspectorObjects.SwapCameras)
        {
            CameraContolTrigger.customInspectorObjects.cameraOnLeft = (CinemachineVirtualCamera)EditorGUILayout.ObjectField("Camera On Left", CameraContolTrigger.customInspectorObjects.cameraOnLeft, typeof(CinemachineVirtualCamera), true);
            CameraContolTrigger.customInspectorObjects.cameraOnRight = (CinemachineVirtualCamera)EditorGUILayout.ObjectField("Camera On Right", CameraContolTrigger.customInspectorObjects.cameraOnRight, typeof(CinemachineVirtualCamera), true);
        }
        if (CameraContolTrigger.customInspectorObjects.PanCameraOnContact)
        {
            CameraContolTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Pan Direction", CameraContolTrigger.customInspectorObjects.panDirection);
            CameraContolTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", CameraContolTrigger.customInspectorObjects.panDistance);
            CameraContolTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", CameraContolTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(CameraContolTrigger);
        }
    } // end OnInspectorGUI

} // end class MyScriptEditor
#endif
