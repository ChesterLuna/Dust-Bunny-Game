using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;

    [SerializeField] GameObject _pauseMenuUI;
    [SerializeField] GameObject _promptQuitGameUI;
    [SerializeField] GameObject _promptQuitMenuUI;
    [SerializeField] GameObject _infoUI;
    [SerializeField] GameObject _rebindUI;
    [SerializeField] GameObject _settingsUI;
    [SerializeField] GameObject _gameplayOverlayUI;
    [SerializeField] GameObject _audioSettingsUI;
    [SerializeField] GameObject _graphicsSettingsUI;


    private float _timeSinceLastResume = 0.0f;

    void Start()
    {
        SetMenu(PauseMenuPage.Gameplay);
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInput.instance.Gather(PlayerStates.Paused).MenuDown && _timeSinceLastResume > 0.3f)
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

        if (!GameIsPaused)
        {
            _timeSinceLastResume += Time.unscaledDeltaTime;
        }
    } // end Update

    public void SetPauseMenuInt(int type)
    {
        if (type < 0 || type >= Enum.GetValues(typeof(PauseMenuPage)).Length)
        {
            Debug.LogError("Invalid PauseMenuPage type.");
            return;
        }
        else if (type == (int)PauseMenuPage.Gameplay)
        {
            Debug.LogWarning("Should not be setting PauseMenuPage to None Via Int.");
            return;
        }
        SetMenu((PauseMenuPage)type);
    }

    public void SetMenu(PauseMenuPage page = PauseMenuPage.Gameplay)

    {
        switch (page)
        {
            case PauseMenuPage.Pause:
                _pauseMenuUI.SetActive(true);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Info:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(true);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Rebind:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(true);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Settings:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(true);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Gameplay:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(true);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Audio:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(true);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.Graphics:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(true);
                break;
            case PauseMenuPage.TrueNone:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.PromptQuitGame:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(true);
                _promptQuitMenuUI.SetActive(false);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
            case PauseMenuPage.PromptQuitMenu:
                _pauseMenuUI.SetActive(false);
                _promptQuitGameUI.SetActive(false);
                _promptQuitMenuUI.SetActive(true);
                _infoUI.SetActive(false);
                _rebindUI.SetActive(false);
                _settingsUI.SetActive(false);
                _gameplayOverlayUI.SetActive(false);
                _audioSettingsUI.SetActive(false);
                _graphicsSettingsUI.SetActive(false);
                break;
        }
    } // end SetMenu

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        SetMenu(PauseMenuPage.Pause);
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
    } // end Pause

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SetMenu(PauseMenuPage.Gameplay);
        _timeSinceLastResume = 0.0f;
        UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
    } // end Resume

    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();
    } // end QuitGame

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    } // end QuitGame

    public void PlayUIPositive()
    {
        if (!GameIsPaused) return;
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
    }

    public void PlayUINavigate()
    {
        if (!GameIsPaused) return;
        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    }

    public void PlayUINegative()
    {
        if (!GameIsPaused) return;
        UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
    }

    public enum PauseMenuPage
    {
        Pause, // 0
        Info, // 1
        Rebind, // 2
        Settings, // 3
        Gameplay, // 4
        Audio, // 5
        Graphics, // 6
        TrueNone, // 7
        PromptQuitGame, // 8
        PromptQuitMenu // 9
    } // end enum PauseMenuPage
} // end PauseMenu
