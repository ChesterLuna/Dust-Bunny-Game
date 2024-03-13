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
    [SerializeField] GameObject _settingsUI;
    [SerializeField] GameObject _gameplayOverlayUI;

    void Start()
    {
        _pauseMenuUI.SetActive(false);
        _infoUI.SetActive(false);
        _rebindUI.SetActive(false);
        _settingsUI.SetActive(false);
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

    public void ToggleTimerDisplay(bool value)
    {
        GameManager.instance.ShowTimer = value;
    }

    public void SetPauseMenuInt(int type)
    {
        if (type < 0 || type > ((int)PauseMenuPage.None))
        {
            Debug.LogError("Invalid PauseMenuPage type.");
            return;
        }
        else if (type == (int)PauseMenuPage.None)
        {
            Debug.LogWarning("Should not be setting PauseMenuPage to None Via Int.");
            return;
        }
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
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                break;
            case PauseMenuPage.Info:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(true);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                break;
            case PauseMenuPage.Rebind:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(true);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                break;
            case PauseMenuPage.Settings:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(true);
                _gameplayOverlayUI.SetActive(false);
                break;
            case PauseMenuPage.None:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(true);
                break;
        }
    } // end SetMenu

    void Pause()
    {
        Debug.Log("Pause Game.");
        Time.timeScale = 0f;
        GameIsPaused = true;
        SetMenu(PauseMenuPage.Pause);
        GameManager.instance?.PauseGameTime();
    } // end Pause

    public void Resume()
    {
        Debug.Log("Resume Game.");
        Time.timeScale = 1f;
        GameIsPaused = false;
        SetMenu(PauseMenuPage.None);
        GameManager.instance?.StartGameTime();
    } // end Resume

    public void QuitGame()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
        Debug.Log("Quit Game.");
        Application.Quit();
    } // end QuitGame

    public enum PauseMenuPage
    {
        Pause, // 0
        Info, // 1
        Rebind, // 2
        Settings, // 3
        None // 4
    }
} // end PauseMenu
