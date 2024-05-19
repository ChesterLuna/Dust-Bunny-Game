using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SpringCleaning.Camera;
public class CameraDialogueManager : MonoBehaviour
{
    public void SetCameraOrthoSize(float sizeModifier)
    {
        CameraManager.Instance.SetOrthographicSize(sizeModifier, true, addValue: true);
    }
}
