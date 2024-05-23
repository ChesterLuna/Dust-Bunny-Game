using System.Collections;
using System.Collections.Generic;
using SpringCleaning.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    private static SceneSwapManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("There are multiple SceneSwapManagers in the scene. Deleting the newest one.");
            Destroy(this);
        }
    }

    public static void ChangeScene(SceneField scene, Vector2? spawnPosition = null)
    {
        // checkpointlocation = spawnPosition;
        _instance.StartCoroutine(_instance.FadeOutThenChangeScene(scene));
    }

    private IEnumerator FadeOutThenChangeScene(SceneField scene)
    {
        SceneFadeManager.Instance.StartFadeOut();
        while (SceneFadeManager.Instance.IsFadingOut) yield return null;

        SceneManager.LoadScene(scene);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneFadeManager.Instance.StartFadeIn();
    }
}
