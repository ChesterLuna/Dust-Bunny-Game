using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modified from https://medium.com/@sujay_reddy/procedural-mesh-in-unity-part-1-lets-draw-a-triangle-4dc03ca7254c

public class TextBubbleLine : MonoBehaviour
{
    Mesh m;
    MeshFilter mf;

    private GameObject currentTarget;

    // Use this for initialization
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        m = new Mesh();
        mf.mesh = m;
    }

    void Update(){
        DrawLineToPoint(currentTarget);
    }

    public void SetTarget(GameObject target){
        currentTarget = target;
        if(target == null){
            currentTarget = gameObject;
        } else {
            Transform manualTargetTransform = target.transform.Find("SpeechBubbleTarget");
            if(manualTargetTransform != null){
                currentTarget = manualTargetTransform.gameObject;
                Debug.Log("Using manual target for " + target.name);
            }
        }
    }

    //This draws a triangle
    void DrawLineToPoint(GameObject target)
    {
        //We need two arrays one to hold the vertices and one to hold the triangles
        Vector3[] VerteicesArray = new Vector3[3];
        int[] trianglesArray = new int[3];

        //lets add 3 vertices in the 3d space

        // find the position of the target
        Vector3 targetPos = transform.position;
        if(target != null){
            targetPos = target.transform.position;
            BoxCollider2D box = target.GetComponent<BoxCollider2D>();
            if(box != null && target.name == "SpeechBubbleTarget"){
                targetPos = box.ClosestPoint(transform.position);
            }
        }

        // find the position of the two other points
        VerteicesArray[0] = new Vector3(
            Mathf.Sin(Vector3.Angle(targetPos, transform.position) + (Mathf.PI/2)), 
            Mathf.Cos(Vector3.Angle(targetPos, transform.position) + (Mathf.PI/2)),
        0);
        VerteicesArray[1] = targetPos - transform.position;
        VerteicesArray[2] = new Vector3(
            Mathf.Sin(Vector3.Angle(targetPos, transform.position) - (Mathf.PI/2)), 
            Mathf.Cos(Vector3.Angle(targetPos, transform.position) - (Mathf.PI/2)),
        0);

        //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
        trianglesArray[0] = 2;
        trianglesArray[1] = 1;
        trianglesArray[2] = 0;

        //add these two triangles to the mesh
        m.vertices = VerteicesArray;
        m.triangles = trianglesArray;
    }
}
