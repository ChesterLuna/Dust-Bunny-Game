using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraDialogueManager : MonoBehaviour
{
    public void SetCameraOrthoSize(float sizeModifier)
    {
        CameraManager.instance.SetOrthographicSize(sizeModifier, true, addValue: true);
    }
}
