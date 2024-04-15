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
            guid = Guid.NewGuid().ToString();
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

    // Used to keep all things related to this guid in one save file
    #region Save and Load
    public void SaveBoolValue(string variableName, bool value)
    {
        ES3.Save(guid + "_" + variableName, value + "/" + guid);
    }

    public bool LoadBoolValue(string variableName, bool defaultValue = false)
    {
        return ES3.Load(guid + "_" + variableName, defaultValue);
    }
    #endregion
}