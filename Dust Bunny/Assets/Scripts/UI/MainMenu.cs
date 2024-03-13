using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public string firstLevel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel);
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
