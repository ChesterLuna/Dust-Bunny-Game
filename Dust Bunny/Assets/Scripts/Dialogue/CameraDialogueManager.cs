using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDialogueManager : MonoBehaviour
{
    public void SetCameraOrthoSize(float sizeModifier){
        CameraManager.instance._cameraOrthographicSizeModifier = sizeModifier;
    }
}
