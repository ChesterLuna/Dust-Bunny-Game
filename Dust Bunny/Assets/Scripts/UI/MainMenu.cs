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
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel);
    }

    public void OpenInfo()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Info Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();
    }
}
