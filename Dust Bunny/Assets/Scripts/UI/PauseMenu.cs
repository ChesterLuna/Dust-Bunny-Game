using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] GameObject _pauseMenuUI;
    [SerializeField] GameObject _infoUI;
    [SerializeField] GameObject _rebindUI;
    [SerializeField] GameObject _timerTextUI;

    void Start()
    {
        _pauseMenuUI.SetActive(false);
        _infoUI.SetActive(false);
        _rebindUI.SetActive(false);
        _timerTextUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInput.instance.ToggleMenu)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    } // end Update

    public void SetPauseMenuInt(int type)
    {
        SetMenu((PauseMenuPage)type);
    }

    public void SetMenu(PauseMenuPage page = PauseMenuPage.None)

    {
        switch (page)
        {
            case PauseMenuPage.Pause:
                _pauseMenuUI.SetActive(true);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                break;
            case PauseMenuPage.Info:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(true);
                _rebindUI.SetActive(false);
                break;
            case PauseMenuPage.Rebind:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(true);
                break;
            case PauseMenuPage.None:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                break;
        }
    } // end SetMenu

    void Pause()
    {
        Debug.Log("Pause Game.");
        Time.timeScale = 0f;
        GameIsPaused = true;
        SetMenu(PauseMenuPage.Pause);
        _timerTextUI.SetActive(false);
        GameManager.instance?.PauseGameTime();
    } // end Pause

    public void Resume()
    {
        Debug.Log("Resume Game.");
        Time.timeScale = 1f;
        GameIsPaused = false;
        SetMenu(PauseMenuPage.None);
        _timerTextUI.SetActive(true);
        GameManager.instance?.StartGameTime();
    } // end Resume

    public enum PauseMenuPage
    {
        Pause,
        Info,
        Rebind,
        None
    }
} // end PauseMenu
