using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    private Vector2 bgDimWorld;
    private Vector2 arenaDimensions;
    private Vector2 bgToArenaRatio;

    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        sprite = GetComponent<SpriteRenderer>();

        // Find background image limits, adjusted for screen size
        screenSize = new Vector2(mainCamera.orthographicSize * mainCamera.aspect, mainCamera.orthographicSize * (1 / mainCamera.aspect));
        bgDimWorld = new Vector2(sprite.size.x * transform.localScale.x, sprite.size.y * transform.localScale.y);
        aspectRatio = sprite.size.y / sprite.size.x;
        arenaDimensions = new Vector2(
            rightBound.transform.position.x - leftBound.transform.position.x,
            bottomBound.transform.position.y - topBound.transform.position.y
        );

        leftLimit = leftBound.transform.position.x - screenSize.x / 2 - bgDimWorld.x / 2;
        rightLimit = rightBound.transform.position.x + screenSize.x / 2 + bgDimWorld.x / 2;
        topLimit = topBound.transform.position.y + screenSize.y / 2 + bgDimWorld.y / 2;
        bottomLimit = bottomBound.transform.position.y - screenSize.y / 2 - bgDimWorld.y / 2;

        Debug.Log("Left parallax limit: " + leftLimit.ToString());
        Debug.Log("Right parallax limit: " + rightLimit.ToString());
        Debug.Log("Top parallax limit: " + topLimit.ToString());
        Debug.Log("Bottom parallax limit: " + bottomLimit.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = mainCamera.transform.position;


        Vector2 percentThroughLevel = new Vector2(
            (cameraPos.x - leftLimit) / (rightLimit - leftLimit),
            (cameraPos.y - topLimit) / (bottomLimit - topLimit));

        Vector3 newPosition = new Vector3(
            cameraPos.x - (percentThroughLevel.x - 0.5f) * bgDimWorld.x,
            cameraPos.y + (percentThroughLevel.y - 0.5f) * bgDimWorld.y,
            1);

        transform.position = newPosition;
    }
}
