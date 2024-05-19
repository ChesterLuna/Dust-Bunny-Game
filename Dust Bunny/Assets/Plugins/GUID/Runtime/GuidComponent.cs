using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

// This component gives a GameObject a stable, non-replicatable Globally Unique IDentifier.
// It can be used to reference a specific instance of an object no matter where it is.
// This can also be used for other systems, such as Save/Load game
[ExecuteInEditMode, DisallowMultipleComponent]
public class GuidComponent : MonoBehaviour, ISerializationCallbackReceiver
{
    // System guid we use for comparison and generation
    System.Guid guid = System.Guid.Empty;

    // Unity's serialization system doesn't know about System.Guid, so we convert to a byte array
    // Fun fact, we tried using strings at first, but that allocated memory and was twice as slow
    [SerializeField]
    private byte[] serializedGuid;


    public bool IsGuidAssigned()
    {
        return guid != System.Guid.Empty;
    }


    // When de-serializing or creating this component, we want to either restore our serialized GUID
    // or create a new one.
    void CreateGuid()
    {
        // if our serialized data is invalid, then we are a new object and need a new GUID
        if (serializedGuid == null || serializedGuid.Length != 16)
        {
#if UNITY_EDITOR
            // if in editor, make sure we aren't a prefab of some kind
            if (IsAssetOnDisk())
            {
                return;
            }
            Undo.RecordObject(this, "Added GUID");
#endif
            guid = System.Guid.NewGuid();
            serializedGuid = guid.ToByteArray();

#if UNITY_EDITOR
            // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
            // force a save of the modified prefab instance properties
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
#endif
        }
        else if (guid == System.Guid.Empty)
        {
            // otherwise, we should set our system guid to our serialized guid
            guid = new System.Guid(serializedGuid);
        }

        // register with the GUID Manager so that other components can access this
        if (guid != System.Guid.Empty)
        {
            if (!GuidManager.Add(this))
            {
                // if registration fails, we probably have a duplicate or invalid GUID, get us a new one.
                serializedGuid = null;
                guid = System.Guid.Empty;
                CreateGuid();
            }
        }
    }

#if UNITY_EDITOR
    private bool IsEditingInPrefabMode()
    {
        if (EditorUtility.IsPersistent(this))
        {
            // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
            return true;
        }
        else
        {
            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(gameObject);
            if (currentStage != mainStage)
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                if (prefabStage != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsAssetOnDisk()
    {
        return PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode();
    }
#endif

    // We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        // This lets us detect if we are a prefab instance or a prefab asset.
        // A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
        if (IsAssetOnDisk())
        {
            serializedGuid = null;
            guid = System.Guid.Empty;
        }
        else
#endif
        {
            if (guid != System.Guid.Empty)
            {
                serializedGuid = guid.ToByteArray();
            }
        }
    }

    // On load, we can go head a restore our system guid for later use
    public void OnAfterDeserialize()
    {
        if (serializedGuid != null && serializedGuid.Length == 16)
        {
            guid = new System.Guid(serializedGuid);
        }
    }

    void Awake()
    {
        CreateGuid();
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
        // at a time that lets us detect what we are
        if (IsAssetOnDisk())
        {
            serializedGuid = null;
            guid = System.Guid.Empty;
        }
        else
#endif
        {
            CreateGuid();
        }
    }

    // Never return an invalid GUID
    public System.Guid GetGuid()
    {
        if (guid == System.Guid.Empty && serializedGuid != null && serializedGuid.Length == 16)
        {
            guid = new System.Guid(serializedGuid);
        }

        return guid;
    }

    // Let the manager know we are gone, so other objects no longer find this
    public void OnDestroy()
    {
        GuidManager.Remove(guid);
    }

    // Wrappers for ES3 Save and load methods
    #region GUID.ES3.Save
    // <summary>Saves the value to the default file with the given key.</summary>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    public void Save(string key, object value)
    {
        ES3.Save(this.GetGuid() + "-" + key, value);
    }

    /// <summary>Saves the value to a file with the given key.</summary>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to store our value to.</param>
    public void Save(string key, object value, string filePath)
    {
        ES3.Save(this.GetGuid() + "-" + key, value, filePath);
    }

    /// <summary>Saves the value to a file with the given key.</summary>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to store our value to.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public void Save(string key, object value, string filePath, ES3Settings settings)
    {
        ES3.Save(this.GetGuid() + "-" + key, value, filePath, settings);
    }

    /// <summary>Saves the value to a file with the given key.</summary>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public void Save(string key, object value, ES3Settings settings)
    {
        ES3.Save(this.GetGuid() + "-" + key, value, settings);
    }

    /// <summary>Saves the value to the default file with the given key.</summary>
    /// <param name="T">The type of the data that we want to save.</param>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    public void Save<T>(string key, T value)
    {
        ES3.Save<T>(this.GetGuid() + "-" + key, value);
    }

    /// <summary>Saves the value to a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to save.</param>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to store our value to.</param>
    public void Save<T>(string key, T value, string filePath)
    {
        ES3.Save<T>(this.GetGuid() + "-" + key, value, filePath);
    }

    /// <summary>Saves the value to a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to save.</param>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to store our value to.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public void Save<T>(string key, T value, string filePath, ES3Settings settings)
    {
        ES3.Save<T>(this.GetGuid() + "-" + key, value, filePath, settings);
    }

    /// <summary>Saves the value to a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to save.</param>
    /// <param name="key">The key we want to use to identify our value in the file.</param>
    /// <param name="value">The value we want to save.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public void Save<T>(string key, T value, ES3Settings settings)
    {
        ES3.Save<T>(this.GetGuid() + "-" + key, value, settings);
    }
    #endregion

    #region GUID.ES3.Load
    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    public object Load(string key)
    {
        return ES3.Load(this.GetGuid() + "-" + key);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    public object Load(string key, string filePath)
    {
        return ES3.Load(this.GetGuid() + "-" + key, filePath);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public object Load(string key, string filePath, ES3Settings settings)
    {
        return ES3.Load(this.GetGuid() + "-" + key, filePath, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public object Load(string key, ES3Settings settings)
    {
        return ES3.Load(this.GetGuid() + "-" + key, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    public object Load(string key, object defaultValue)
    {
        return ES3.Load(this.GetGuid() + "-" + key, defaultValue);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    public object Load(string key, string filePath, object defaultValue)
    {
        return ES3.Load(this.GetGuid() + "-" + key, filePath, defaultValue);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public object Load(string key, string filePath, object defaultValue, ES3Settings settings)
    {
        return ES3.Load(this.GetGuid() + "-" + key, filePath, defaultValue, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public object Load(string key, object defaultValue, ES3Settings settings)
    {
        return ES3.Load(this.GetGuid() + "-" + key, defaultValue, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    public T Load<T>(string key)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    public T Load<T>(string key, string filePath)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, filePath);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public T Load<T>(string key, string filePath, ES3Settings settings)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, filePath, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public T Load<T>(string key, ES3Settings settings)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    public T Load<T>(string key, T defaultValue)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, defaultValue);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    public T Load<T>(string key, string filePath, T defaultValue)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, filePath, defaultValue);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="filePath">The relative or absolute path of the file we want to load from.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public T Load<T>(string key, string filePath, T defaultValue, ES3Settings settings)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, filePath, defaultValue, settings);
    }

    /// <summary>Loads the value from a file with the given key.</summary>
    /// <param name="T">The type of the data that we want to load.</param>
    /// <param name="key">The key which identifies the value we want to load.</param>
    /// <param name="defaultValue">The value we want to return if the file or key does not exist.</param>
    /// <param name="settings">The settings we want to use to override the default settings.</param>
    public T Load<T>(string key, T defaultValue, ES3Settings settings)
    {
        return ES3.Load<T>(this.GetGuid() + "-" + key, defaultValue, settings);
    }
    #endregion
}
