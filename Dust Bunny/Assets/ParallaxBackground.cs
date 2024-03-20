using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject leftBound;
    public GameObject rightBound;

    public GameObject topBound;
    public GameObject bottomBound;

    private Vector3 startPos;

    private SpriteRenderer sprite;

    private float leftLimit;
    private float rightLimit;
    private float topLimit;
    private float bottomLimit;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        sprite = GetComponent<SpriteRenderer>();

        // Find background image limits, adjusted for screen size
        Vector2 screenSize = new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * (1/mainCamera.aspect));
        leftLimit = leftBound.transform.position.x - screenSize.x;
        rightLimit = rightBound.transform.position.x + screenSize.x;
        topLimit = topBound.transform.position.y - screenSize.y;
        bottomLimit = bottomBound.transform.position.y + screenSize.y;

        Debug.Log(screenSize);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        Vector2 backgroundDimensionsWorld = new Vector2(sprite.size.x * transform.localScale.x, sprite.size.y * transform.localScale.y);
        float aspectRatio = sprite.size.y / sprite.size.x;
        
        Vector2 percentThroughLevel = new Vector2(
            (cameraPos.x - leftLimit) / (rightLimit - leftLimit), 
            (cameraPos.y - topLimit)  / (bottomLimit - topLimit));

        Vector3 newPosition = new Vector3(
            cameraPos.x - (percentThroughLevel.x - 0.5f) * backgroundDimensionsWorld.x, 
            cameraPos.y + (percentThroughLevel.y - 0.5f) * backgroundDimensionsWorld.y,
            1);
        
        transform.position = newPosition;
    }
}
