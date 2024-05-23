using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

// We only want to disable the SettingsMenuHub after all other scripts have had a chance to run their Start methods and the proper values have been loaded from playerprefs).
[DefaultExecutionOrder(1)]
public class SettingsMenuHub : MonoBehaviour
{
    private static SettingsMenuHub _instance;

    [Title("Settings Menu")]
    [SerializeField, Required, Tooltip("This is what will be enabled/disabled")] private GameObject _settingsMenuObject;
    [SerializeField, Required] Toggle _displayTimerToggle;
    [SerializeField, Required] private Button _backButton;

    [Title("Rebinds")]
    [SerializeField, Required] Button _rebindButton;
    [SerializeField, Required] private GameObject _rebindMenuObject;

    [Title("Audio")]
    [SerializeField, Required] private Button _audioButton;
    [SerializeField, Required] private GameObject _audioMenuObject;

    [Title("Graphics")]
    [SerializeField, Required] private Button _graphicsButton;
    [SerializeField, Required] private GameObject _graphicsMenuObject;

    private Action _callbackAction;
    private GameObject _callbackMenu;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("There are multiple TabMenus in the scene. Deleting the newest one.");
            Destroy(this);
        }
    }

    private void Start()
    {
        _rebindMenuObject.SetActive(false);
        _rebindButton.onClick.AddListener(() => OpenChildSettingsMenu(_rebindMenuObject));

        _audioMenuObject.SetActive(false);
        _audioButton.onClick.AddListener(() => OpenChildSettingsMenu(_audioMenuObject));

        _graphicsMenuObject.SetActive(false);
        _graphicsButton.onClick.AddListener(() => OpenChildSettingsMenu(_graphicsMenuObject));

        _settingsMenuObject.SetActive(false);
        _backButton.onClick.AddListener(() => CloseSettingsHubMenu());
    }

    void OnEnable()
    {
        _displayTimerToggle.isOn = GameManager.Instance.ShowTimer;
        _displayTimerToggle.onValueChanged.AddListener((value) => GameManager.Instance.SetShowTimer(value));
    } // end OnEnable

    void OnDisable()
    {
        _displayTimerToggle.onValueChanged.RemoveAllListeners();
    } // end OnDisable

    public static void OpenChildSettingsMenu(GameObject childMenu)
    {
        _instance._rebindMenuObject.SetActive(false);
        _instance._audioMenuObject.SetActive(false);
        _instance._graphicsMenuObject.SetActive(false);

        _instance._settingsMenuObject.SetActive(false);

        childMenu.SetActive(true);
    }

    public static void ReturnToSettingsHubMenu()
    {
        OpenChildSettingsMenu(_instance._settingsMenuObject);
    }

    #region Open/Close SettingsHubMenu
    /// <summary>
    /// Open the SettingsHubMenu and optinally set a callback action (called on menu close) and a callback menu (which will be enabled on menu close).
    /// The calling menu is expected to disable itself after the SettingsHubMenu is opened.
    /// </summary>
    /// <param name="callbackMenu"></param>
    /// <param name="callbackAction"></param>
    public static void OpenSettingsHubMenu(GameObject callbackMenu = null, Action callbackAction = null)
    {
        _instance._callbackAction = callbackAction;
        _instance._callbackMenu = callbackMenu;

        _instance._settingsMenuObject.SetActive(true);
    }

    /// <summary>
    /// Dsiables the SettingsHubMenu, enables the callback menu if it is set, and calls the callback action if it is set.
    /// </summary>
    public static void CloseSettingsHubMenu()
    {
        if (_instance._callbackMenu != null) _instance._callbackMenu.SetActive(true);
        if (_instance._callbackAction != null) _instance._callbackAction();

        // Safety check to ensure that the callback action and menu are not called multiple times.
        _instance._callbackAction = null;
        _instance._callbackMenu = null;

        _instance._rebindMenuObject.SetActive(false);
        _instance._audioMenuObject.SetActive(false);
        _instance._graphicsMenuObject.SetActive(false);

        _instance._settingsMenuObject.SetActive(false);
    }
    #endregion
}

