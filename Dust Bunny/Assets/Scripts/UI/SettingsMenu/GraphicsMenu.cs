using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GraphicsMenu : MonoBehaviour
{
    private List<Resolution> _resolutions = new List<Resolution>();
    [Required, SerializeField] private TMPro.TMP_Dropdown _resolutionDropdown;
    [Required, SerializeField] private TMPro.TMP_Dropdown _qualityDropdown;
    private List<string> _windowOptionNames = new List<string>();
    private List<FullScreenMode> _windowOptionValues = new List<FullScreenMode>();
    [Required, SerializeField] private TMPro.TMP_Dropdown _windowDropdown;
    [Required, SerializeField] private TMPro.TMP_Dropdown _FPSDropdown;
    [Required, SerializeField] private List<int> _FPSOptions = new List<int> { 30, 60, 120, 144, 240, -1 };
    [Required, SerializeField] private Toggle _vSyncToggle;

    private bool _setup = false;
    void Start()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt("FPS", Application.targetFrameRate);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync", QualitySettings.vSyncCount);

        // Setup Dropdowns
        SetupResolutionDropdown();
        SetupQualityDropdown();
        SetupFPSDropDown();
        SetupWindowDropdown();

        // Setup Toggles
        _vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        _setup = true;

    } // end Start

    #region FPS
    private void SetupFPSDropDown()
    {
        _FPSDropdown.ClearOptions();
        int index = 0;
        List<string> options = new List<string>();
        for (int i = 0; i < _FPSOptions.Count; i++)
        {
            if (_FPSOptions[i] == Application.targetFrameRate)
            {
                index = i;
            }
            if (_FPSOptions[i] == -1)
            {
                options.Add("Unlimited");
            }
            else
            {
                options.Add(_FPSOptions[i].ToString());
            }
        }
        _FPSDropdown.AddOptions(options);
        _FPSDropdown.value = index;
        _FPSDropdown.RefreshShownValue();
    } // end SetupFPSDropDown

    public void SetFPS(int fpsIndex)
    {
        if (!_setup) return;
        int fps = _FPSOptions[fpsIndex];
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt("FPS", Application.targetFrameRate);
        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetFPS

    #endregion

    #region  Resolution
    private void SetupResolutionDropdown()
    {
        _resolutions = Screen.resolutions.Where(resolution => resolution.refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value).ToList();

        _resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = _resolutions.Count - 1;

        for (int i = 0; i < _resolutions.Count; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);
            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
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
        if (!_setup) return;
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);

        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
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
        if (!_setup) return;
        QualitySettings.SetQualityLevel(qualityIndex);
        QualitySettings.vSyncCount = _vSyncToggle.isOn ? 1 : 0;

        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetQuality
    #endregion

    #region Window
    private void SetupWindowDropdown()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            _windowOptionNames.Add("Exclusive Fullscreen");
            _windowOptionValues.Add(FullScreenMode.ExclusiveFullScreen);
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            _windowOptionNames.Add("Maximized Window");
            _windowOptionValues.Add(FullScreenMode.MaximizedWindow);
        }
        _windowOptionNames.Add("Fullscreen Window");
        _windowOptionValues.Add(FullScreenMode.FullScreenWindow);

        _windowOptionNames.Add("Windowed");
        _windowOptionValues.Add(FullScreenMode.Windowed);


        _windowDropdown.ClearOptions();
        _windowDropdown.AddOptions(_windowOptionNames);
        _windowDropdown.value = _windowOptionValues.FindIndex(x => x == Screen.fullScreenMode);
        _windowDropdown.RefreshShownValue();
    } // end SetupWindowDropdown

    public void SetWindow(int windnowIndex)
    {
        if (!_setup) return;

        FullScreenMode fullscreenMode = _windowOptionValues[windnowIndex];
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreenMode, Screen.currentResolution.refreshRateRatio);
        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetFullscreen
    #endregion

    public void SetVSync(bool isVSync)
    {
        if (!_setup) return;
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        // Save the vSync to the player prefs
        PlayerPrefs.SetInt("vSync", QualitySettings.vSyncCount);

        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetVSync

} // end Class GraphicsMenu