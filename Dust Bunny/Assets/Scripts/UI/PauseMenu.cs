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
    [SerializeField] GameObject _audioSettingsUI;

    void Start()
    {
        _pauseMenuUI.SetActive(false);
        _infoUI.SetActive(false);
        _rebindUI.SetActive(false);
        _settingsUI.SetActive(false);
        _audioSettingsUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInput.instance.Gather().MenuDown)
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
        if (type < 0 || type >= Enum.GetValues(typeof(PauseMenuPage)).Length)
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
                _audioSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Info:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(true);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Rebind:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(true);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Settings:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(true);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.None:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(true);
                _audioSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Audio:
                _pauseMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(true);
                break;
        }
    } // end SetMenu

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        SetMenu(PauseMenuPage.Pause);
        GameManager.instance?.PauseGameTime();
        UserInput.instance.gameObject.SetActive(false);
    } // end Pause

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SetMenu(PauseMenuPage.None);
        GameManager.instance?.StartGameTime();
        UserInput.instance.gameObject.SetActive(true);
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
        None, // 4
        Audio // 5
    } // end enum PauseMenuPage
} // end PauseMenu
