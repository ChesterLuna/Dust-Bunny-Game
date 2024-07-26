using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;

    public GameObject initialButton;

    void Start(){
        StartCoroutine(SetPlayButtonEnabled());
    }

    IEnumerator SetPlayButtonEnabled(){
        yield return new WaitForSeconds(0.5f);
        EventSystem.current.firstSelectedGameObject = initialButton;
        Selectable s = initialButton.GetComponent<Selectable>();
        s.Select();
    }

    public void StartGame()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevelByString(firstLevel, "CrossFade", 1.0f);
        GameManager.instance.ResetGameTime();
        GameManager.instance.StartGameTime();
    }

    public void CustomTransition(string sceneName)
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void OpenInfo()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevelByString("Info Menu", "CrossFade", 1.0f);
    }

    public void QuitGame()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
        Debug.Log("Quit Game.");
        Application.Quit();
    }
}
