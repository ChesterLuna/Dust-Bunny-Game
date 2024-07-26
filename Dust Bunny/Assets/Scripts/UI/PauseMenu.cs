using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [SerializeField] Button _initialButton;
    [SerializeField] Button _initialKeyboardButton;
    [SerializeField] Button _initialAudioButton;
    [SerializeField] Button _initialSettingsButton;
    [SerializeField] Button _initialGraphicsButton;
    [SerializeField] Button _initialInfoButton;
    [SerializeField] Button _initialQuitToMenuButton;
    [SerializeField] Button _initialQuitGameButton;
    [SerializeField] GameObject restartButtonToDisable;
    [SerializeField] GameObject aboveRestartButton;
    [SerializeField] GameObject belowRestartButton;


    private float _timeSinceLastResume = 0.0f;
    private float _timeSinceLastPageFlip = 0.0f;
    private bool _wasDirectToSettings = false;

    void Start()
    {
        SetMenu(PauseMenuPage.Gameplay);

        RemoveRestartButtonInInvalidScenes();
    }

    void RemoveRestartButtonInInvalidScenes(){
        // Unable to reload the main menu or burrow
        if(SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Burrow-NEW"){
            restartButtonToDisable.SetActive(false);

            //Reassign navigation targets
            //Create a new navigation
            Selectable aboveAboverestartButton = aboveRestartButton.GetComponent<Button>().navigation.selectOnUp;
            Navigation NewNav = new Navigation();
            NewNav.mode = Navigation.Mode.Explicit;
            NewNav.selectOnUp = aboveAboverestartButton;
            NewNav.selectOnDown = belowRestartButton.GetComponent<Button>();
            aboveRestartButton.GetComponent<Button>().navigation = NewNav;

            //Create a new navigation
            Selectable belowBelowRestartButton = belowRestartButton.GetComponent<Button>().navigation.selectOnDown;
            NewNav = new Navigation();
            NewNav.mode = Navigation.Mode.Explicit;
            NewNav.selectOnUp = aboveRestartButton.GetComponent<Button>();
            NewNav.selectOnDown = belowBelowRestartButton;
            belowRestartButton.GetComponent<Button>().navigation = NewNav;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInput.instance != null && UserInput.instance.Gather(PlayerStates.Paused).MenuDown && _timeSinceLastResume > 0.3f)
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
            _timeSinceLastPageFlip += Time.unscaledDeltaTime;
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

                _initialButton.Select();
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

                _initialInfoButton.Select();
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

                _initialKeyboardButton.Select();
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

                _initialSettingsButton.Select();
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

                StartCoroutine(ResetButton());
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

                _initialAudioButton.Select();
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

                _initialGraphicsButton.Select();
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

                _initialQuitGameButton.Select();
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

                _initialQuitToMenuButton.Select();
                break;
        }
    } // end SetMenu

    IEnumerator ResetButton(){
        yield return new WaitForSeconds(0.2f);
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
    }

    void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        SetMenu(PauseMenuPage.Pause);
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        _wasDirectToSettings = false;
    } // end Pause

    public void DirectToSettings()
    {
        StartCoroutine(OpenSettings());
    }

    IEnumerator OpenSettings(){
        yield return new WaitForSeconds(0.2f);
        SetMenu(PauseMenuPage.Settings);
        _wasDirectToSettings = true;
    }

    public void SettingsMenuBack(){
        if(_wasDirectToSettings){
            Resume();
        } else {
            SetPauseMenuInt(0);
        }
    }

    public void Resume()
    {
        UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
        Time.timeScale = 1f;
        GameIsPaused = false;
        SetMenu(PauseMenuPage.Gameplay);
        _timeSinceLastResume = 0.0f;
        
    } // end Resume

    public void QuitGame()
    {
        Debug.Log("Quit Game.");
        Application.Quit();
    } // end QuitGame

    public void QuitToMenu()
    {
        Resume();
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevelByString("Main Menu", "CrossFade", 1.0f);
        //SceneManager.LoadScene("Main Menu");
    } // end QuitGame

    public void RestartLevel(){
        // Unable to reload the main menu
        if(SceneManager.GetActiveScene().name == "Main Menu"){
            return;
        }

        // Unable to reload the burrow - this is really stupid but its 3am
        if(SceneManager.GetActiveScene().name == "Burrow-NEW"){
            return;
        }

        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevelByString(SceneManager.GetActiveScene().name, "CrossFade", 1.0f);
        Resume();
    }

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
