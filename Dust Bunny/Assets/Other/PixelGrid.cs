using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PixelGrid : EditorWindow
{
    [MenuItem("Window/Pixel Grid")]
    public static void ShowWindow()
    {
        EditorWindow ew = EditorWindow.GetWindow(typeof(PixelGrid));
        ew.minSize = new Vector2(200, 116);
        ew.maxSize = new Vector2(200, 116);
    }
    
    private float pixelsPerUnit = 48;
    private float gridSize = 8;

    private bool autoSnap = false;
    private float unitScale;
    private Transform[] trans;

    public void OnEnable()
    {
        unitScale = 1 / pixelsPerUnit;
        trans = Selection.transforms;
    }

    public void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Pixels Per Unit : ");
        pixelsPerUnit = EditorGUILayout.FloatField(pixelsPerUnit);
        if(GUI.changed)
            unitScale = 1 / pixelsPerUnit;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Grid Pixel Size : ");
        gridSize = EditorGUILayout.FloatField(gridSize);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Auto Snap : ");
        autoSnap = EditorGUILayout.Toggle(autoSnap);
        if (GUI.changed)
            unitScale = 1 / pixelsPerUnit;
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Unit Scale : " + unitScale);
        GUILayout.Label("Selected : " + trans.Length + " objects.");

        EditorGUI.BeginDisabledGroup(autoSnap);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap X", "buttonleft"))
            SnapXToGrid();
        if (GUILayout.Button("Snap Y", "buttonmid"))
            SnapYToGrid();
        if (GUILayout.Button("Snap All", "buttonright"))
            SnapAllToGrid();
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }

    private void Update()
    {
        if (Selection.transforms.Length != trans.Length)
        {
            trans = Selection.transforms;
            Repaint();
        }
        if (!autoSnap)
            return;

        SnapAllToGrid();
    }

    void SnapAllToGrid()
    {
        SnapXToGrid();
        SnapYToGrid();
    }

    void SnapXToGrid()
    {
        if (trans == null || trans.Length == 0)
        {
            trans = Selection.transforms;
            if (trans == null || trans.Length == 0)
                return;
        }
        foreach (Transform t in trans)
        {
            if (t.gameObject.activeInHierarchy)
            {
                Vector3 pos = t.position;
                float newX = SnapToGrid(pos.x);
                t.position = new Vector2(newX, pos.y);
            }
        }        
    }

    void SnapYToGrid()
    {
        if (trans == null || trans.Length == 0)
        {
            trans = Selection.transforms;
            if (trans == null || trans.Length == 0)
                return;
        }

        foreach (Transform t in trans)
        {
            if (t.gameObject.activeInHierarchy)
            {
                Vector3 pos = t.position;
                float newY = SnapToGrid(pos.y);
                t.position = new Vector2(pos.x, newY);
            }
        }        
    }

    float SnapToGrid(float value)
    {
        float scale = gridSize * unitScale;
        float firstPass = Mathf.Round(value / scale) * scale;
        float newValue = Mathf.Round(firstPass / unitScale) * unitScale;
        return newValue;
    }
}
