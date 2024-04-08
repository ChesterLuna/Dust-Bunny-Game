using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleByCameraZoom : MonoBehaviour
{
    public bool useInitialScale;
    public Vector3 otherwiseInitialScale;

    public bool useInitialCameraZoom;
    public float otherwiseReferenceCameraZoom;

    private Vector3 _initialScale;
    private float _initialCameraSize;
    // Start is called before the first frame update
    void Start()
    {
        if (useInitialScale){
            _initialScale = transform.localScale;
        } else {
            _initialScale = otherwiseInitialScale;
        }

        if (useInitialCameraZoom) {
            _initialCameraSize = Camera.main.orthographicSize;
        } else {
            _initialCameraSize = otherwiseReferenceCameraZoom;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = Camera.main.orthographicSize / _initialCameraSize;
        Vector3 newScale = _initialScale * ratio;
        transform.localScale = newScale;
    }
}
