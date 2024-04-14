using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GraphicsMenu : MonoBehaviour
{
    private Resolution[] _resolutions;
    [SerializeField] private TMPro.TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMPro.TMP_Dropdown _qualityDropdown;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Toggle _vSyncToggle;
    [SerializeField] private TMPro.TMP_Dropdown _FPSDropdown;
    [SerializeField] private List<int> _FPSOptions = new List<int> { 30, 60, 120, 144, 240, -1 };

    private bool _setup = false;

    void Start()
    {
        Debug.Log("Target1 " + Application.targetFrameRate);

        Debug.Log(PlayerPrefs.GetInt("ScreenWidth") + " " + PlayerPrefs.GetInt("ScreenHeight") + " " + PlayerPrefs.GetInt("FPS") + " " + PlayerPrefs.GetInt("vSync") + " " + PlayerPrefs.GetInt("Quality") + " " + PlayerPrefs.GetInt("FullScreen"));
        Debug.Log(PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width) + " " + PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height) + " " + PlayerPrefs.GetInt("FPS", Application.targetFrameRate) + " " + PlayerPrefs.GetInt("vSync", QualitySettings.vSyncCount) + " " + PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel()) + " " + PlayerPrefs.GetInt("FullScreen", Screen.fullScreen ? 1 : 0));
        // Load the values from the player prefs only if they exist, esle use the defaults
        Screen.fullScreen = PlayerPrefs.GetInt("FullScreen", Screen.fullScreen ? 1 : 0) == 1;
        Screen.SetResolution(PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width), PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height), Screen.fullScreen);
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", Application.targetFrameRate);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync", QualitySettings.vSyncCount);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel()));
        Debug.Log("Target2 " + Application.targetFrameRate);

        // Setup Dropdowns
        SetupResolutionDropdown();
        SetupQualityDropdown();
        SetupFPSDropDown();

        // Setup Toggles
        _fullscreenToggle.isOn = Screen.fullScreen;
        _vSyncToggle.isOn = QualitySettings.vSyncCount > 0;


        _setup = true;
    } // end Start

    #region FPS
    private void SetupFPSDropDown()
    {
        _FPSDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (int fps in _FPSOptions)
        {
            if (fps == -1)
            {
                options.Add("Unlimited");
            }
            else
            {
                options.Add(fps.ToString());
            }
        }
        _FPSDropdown.AddOptions(options);
        int index = 0;
        Debug.Log("Target " + Application.targetFrameRate);
        for (int i = 0; i < _FPSOptions.Count; i++)
        {
            if (_FPSOptions[i] == Application.targetFrameRate)
            {
                index = i;
                break;
            }
        }

        Debug.Log(index);
        _FPSDropdown.value = index;
        SetFPS(index);
        _FPSDropdown.RefreshShownValue();
    } // end SetupFPSDropDown

    public void SetFPS(int fpsIndex)
    {
        int fps = _FPSOptions[fpsIndex];
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt("FPS", Application.targetFrameRate);
        if (_setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetFPS

    #endregion

    #region  Resolution
    private void SetupResolutionDropdown()
    {
        _resolutions = Screen.resolutions;

        _resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height + " @ " + _resolutions[i].refreshRateRatio + "Hz";
            options.Add(option);
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();
    } // end SetupResolutionDropdown

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        // Save the resolution to the player prefs
        PlayerPrefs.SetInt("ScreenWidth", resolution.width);
        PlayerPrefs.SetInt("ScreenHeight", resolution.height);

        if (_setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetResolution
    #endregion

    #region Quality
    private void SetupQualityDropdown()
    {
        _qualityDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            options.Add(QualitySettings.names[i]);
        }
        _qualityDropdown.AddOptions(options);
        _qualityDropdown.value = QualitySettings.GetQualityLevel();
        _qualityDropdown.RefreshShownValue();
    } // end SetupQualityDropdown

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        // Save the quality to the player prefs
        PlayerPrefs.SetInt("Quality", QualitySettings.GetQualityLevel());

        if (_setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetQuality
    #endregion

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        // Save the fullscreen to the player prefs
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0);

        if (_setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetFullscreen

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        // Save the vSync to the player prefs
        PlayerPrefs.SetInt("vSync", QualitySettings.vSyncCount);

        if (_setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetVSync

} // end Class GraphicsMenu
