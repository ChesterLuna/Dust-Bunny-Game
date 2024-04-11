using System;
using UnityEditor;
using UnityEngine;


// From https://bdts.com.au/tips-and-resources/unity-how-to-save-and-load-references-to-gameobjects.html
// This script is used to generate a unique ID for each object in the scene
[ExecuteInEditMode]
public class PersistentGUID : MonoBehaviour
{
    public string guid;

#if UNITY_EDITOR

    /// <summary>
    /// Create a new unique ID for this object when it's created
    /// </summary>
    private void Awake()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            guid = "";
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
    }

    /// <summary>
    /// This is only needed if you are adding this script to prefabs that already have instances of in your scenes
    /// - This will update any object that doesn't already have a guid with one.
    /// </summary>
    private void Update()
    {
        if (String.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString();
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
    }
#endif
}