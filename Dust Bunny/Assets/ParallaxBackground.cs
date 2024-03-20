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
    private float aspectRatio;
    private Vector2 screenSize;
    private Vector2 backgroundDimensionsWorld;
    private Vector2 arenaDimensions;
    private Vector2 bgToArenaRatio;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        sprite = GetComponent<SpriteRenderer>();

        // Find background image limits, adjusted for screen size
        screenSize = new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * (1/mainCamera.aspect));
        backgroundDimensionsWorld = new Vector2(sprite.size.x * transform.localScale.x, sprite.size.y * transform.localScale.y);
        aspectRatio = sprite.size.y / sprite.size.x;
        arenaDimensions = new Vector2(
            rightBound.transform.position.x - leftBound.transform.position.x,
            bottomBound.transform.position.y - topBound.transform.position.y
        );

        leftLimit = leftBound.transform.position.x;
        rightLimit = rightBound.transform.position.x;
        topLimit = topBound.transform.position.y;
        bottomLimit = bottomBound.transform.position.y;

        Debug.Log(screenSize);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        
        
        Vector2 percentThroughLevel = new Vector2(
            (cameraPos.x - leftLimit) / (rightLimit - leftLimit), 
            (cameraPos.y - topLimit)  / (bottomLimit - topLimit));

        Vector3 newPosition = new Vector3(
            cameraPos.x - (percentThroughLevel.x - 0.5f) * backgroundDimensionsWorld.x - screenSize.x, 
            cameraPos.y + (percentThroughLevel.y - 0.5f) * backgroundDimensionsWorld.y - screenSize.y, 
            1);
        
        transform.position = newPosition;
    }
}
