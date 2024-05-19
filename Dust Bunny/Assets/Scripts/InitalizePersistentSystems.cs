using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to create a persistent game object housing all the systems/managers that need to persist between scenes.
/// </summary>
public class InitalizePersistentSystems : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Execute()
    {
        GameObject PersistentSystems = Instantiate(Resources.Load<GameObject>("General/PersistentSystems"));
        Object.DontDestroyOnLoad(PersistentSystems);
    }
}
