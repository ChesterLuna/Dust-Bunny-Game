using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpringCleaning.Player;
using Sirenix.OdinInspector;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    private bool _gameIsPaused = false;
    [Title("Pause Menu")]
    [SerializeField, Required] GameObject _pauseMenuUI;
    [SerializeField, Required] Button _resumeButton;
    [Title("Settings Menu")]
    [SerializeField, Required] Button _settingsMenuButton;
    // [Title("Prompt Quit Game")]
    // [SerializeField, Required] GameObject _promptQuitGameUI;
    // [SerializeField, Required] Button _quitGameButton;
    [Title("Prompt Quit Menu")]
    [SerializeField, Required] GameObject _promptQuitMenuUI;
    [SerializeField, Required] Button _quitMenuButton;
    [Title("Info")]
    [SerializeField, Required] GameObject _infoUI;
    [SerializeField, Required] Button _infoButton;
    [Title("Gameplay UI")]
    [SerializeField, Required] GameObject _gameplayUI;

    private float _timeSinceLastResume = 0.0f;

    void Start()
    {
        _gameIsPaused = false;
        _pauseMenuUI.SetActive(false);
        _resumeButton.onClick.AddListener(() => Resume());

        _settingsMenuButton.onClick.AddListener(() => OpenSettingsMenu());

        // _promptQuitGameUI.SetActive(false);
        // _quitGameButton.onClick.AddListener(() => OpenChildPauseMenu(_promptQuitGameUI));

        _promptQuitMenuUI.SetActive(false);
        _quitMenuButton.onClick.AddListener(() => OpenChildPauseMenu(_promptQuitMenuUI));

        _infoUI.SetActive(false);
        _infoButton.onClick.AddListener(() => OpenChildPauseMenu(_infoUI));

        _gameplayUI.SetActive(true);
    }

    public void OpenSettingsMenu()
    {
        SettingsMenuHub.OpenSettingsHubMenu(callbackMenu: _pauseMenuUI);
        _pauseMenuUI.SetActive(false);
    }

    public void OpenChildPauseMenu(GameObject childMenu)
    {
        // _promptQuitGameUI.SetActive(false);
        _promptQuitMenuUI.SetActive(false);
        _infoUI.SetActive(false);
        _pauseMenuUI.SetActive(false);
        _gameplayUI.SetActive(false);

        childMenu.SetActive(true);
    }

    public void ReturnToPauseMenu()
    {
        OpenChildPauseMenu(_pauseMenuUI);
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInput.Instance.Gather(PlayerStates.Paused).MenuDown && _timeSinceLastResume > 0.3f)
        {
            if (_gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (!_gameIsPaused)
        {
            _timeSinceLastResume += Time.unscaledDeltaTime;
        }
    } // end Update

    void Pause()
    {
        Time.timeScale = 0f;
        _gameIsPaused = true;
        OpenChildPauseMenu(_pauseMenuUI);
        UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
    } // end Pause

    public void Resume()
    {
        Time.timeScale = 1f;
        _gameIsPaused = false;
        OpenChildPauseMenu(_gameplayUI);
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
}
