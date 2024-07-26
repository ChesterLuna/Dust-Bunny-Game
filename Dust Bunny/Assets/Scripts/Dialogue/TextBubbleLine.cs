using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modified from https://medium.com/@sujay_reddy/procedural-mesh-in-unity-part-1-lets-draw-a-triangle-4dc03ca7254c

public class TextBubbleLine : MonoBehaviour
{
    MeshFilter[] meshes;
    MeshRenderer[] renderers;

    private GameObject currentTarget;
    private Vector3 currentTargetPos;
    public float speed = 5;
    private bool isRunning = false;
    private float timeUntilVisible = 0;
    private int lineHiddenTurns = 0;
    // Use this for initialization
    void Start()
    {
        meshes = GetComponentsInChildren<MeshFilter>();
        renderers = GetComponentsInChildren<MeshRenderer>();

        foreach(MeshFilter meshFilter in meshes){
            Mesh m = new Mesh();
            meshFilter.mesh = m;
        }

        if(currentTarget == null){
            currentTarget = gameObject;
        }

        currentTargetPos = currentTarget.transform.position;
        
    }

    void Update(){
        if(isRunning){
            UpdateTargetPos(currentTarget);
            DrawLineToPoint(currentTarget);

            timeUntilVisible -= Time.deltaTime;
            if(timeUntilVisible < 0){
                timeUntilVisible = 0;

            }
            foreach(MeshRenderer meshRenderer in renderers){
                meshRenderer.enabled = (timeUntilVisible == 0) && (lineHiddenTurns <= 0);    
            }
        }
    }

    public void OnEnable(){
        timeUntilVisible = 0.1f;
    }

    public void PostCinematic(){
        timeUntilVisible = 0.1f;
        currentTargetPos = currentTarget.transform.position;
        Debug.Log("Post Cinematic Buffer");
    }

    public void HideLine(int hiddenTurns){
        lineHiddenTurns = hiddenTurns;
    }

    public void TickHideTurns(){
        lineHiddenTurns--;
    }

    public void SetRunning(bool isRunning){
        this.isRunning = isRunning;
    }

    public void SetTarget(GameObject target){
        currentTarget = target;
        if(target == null){
            currentTarget = gameObject;
        } else {
            Transform manualTargetTransform = target.transform.Find("SpeechBubbleTarget");
            if(manualTargetTransform != null){
                currentTarget = manualTargetTransform.gameObject;
            }
        }

        // Extra frame update
        UpdateTargetPos(currentTarget);
    }

    private void UpdateTargetPos(GameObject target){
        // find the position of the target
        Vector3 targetPos = transform.position;
        if(target != null){
            targetPos = target.transform.position;
            BoxCollider2D box = target.GetComponent<BoxCollider2D>();
            if(box != null && target.name == "SpeechBubbleTarget"){
                targetPos = box.ClosestPoint(transform.position);
            }
        }
        currentTargetPos = Vector3.Lerp(currentTargetPos, targetPos, Mathf.Min(speed * Time.deltaTime, 1));
    }

    //This draws a triangle
    void DrawLineToPoint(GameObject target)
    {
        Vector3 targetPos = currentTargetPos;

        foreach(MeshFilter mf in meshes){
            Mesh m = mf.mesh;
            //We need two arrays one to hold the vertices and one to hold the triangles
            Vector3[] VerteicesArray = new Vector3[3];
            int[] trianglesArray = new int[3];

            //lets add 3 vertices in the 3d space
            // find the position of the two other points
            VerteicesArray[0] = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad * Vector3.Angle(targetPos, transform.position) + (Mathf.PI/4)), 
                Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(targetPos, transform.position) + (Mathf.PI/4)),
            0);
            VerteicesArray[1] = targetPos - transform.position;
            VerteicesArray[2] = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad * (Vector3.Angle(targetPos, transform.position)) - (Mathf.PI/4)), 
                Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(targetPos, transform.position) - (Mathf.PI/4)),
            0);

            //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
            trianglesArray[0] = 0;
            trianglesArray[1] = 1;
            trianglesArray[2] = 2;

            //add these two triangles to the mesh
            m.vertices = VerteicesArray;
            m.triangles = trianglesArray;
        }
    }
}
