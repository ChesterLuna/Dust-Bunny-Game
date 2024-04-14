using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;

    public void StartGame()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Info Menu");
    }

    public void QuitGame()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
        Debug.Log("Quit Game.");
        Application.Quit();
    }
}
