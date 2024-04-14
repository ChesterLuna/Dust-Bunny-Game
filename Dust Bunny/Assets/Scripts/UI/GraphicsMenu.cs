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


    private bool setup = false;

    void Start()
    {
        SetupResolutionDropdown();
        SetupQualityDropdown();
        SetupFPSDropDown();
        _fullscreenToggle.isOn = Screen.fullScreen;
        _vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        setup = true;
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
        SetFPS(index);
        _FPSDropdown.RefreshShownValue();
    } // end SetupFPSDropDown

    public void SetFPS(int fpsIndex)
    {
        Application.targetFrameRate = _FPSOptions[fpsIndex];
        if (setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
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
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
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
        if (setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
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
        if (setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetQuality
    #endregion

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        if (setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetFullscreen

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        if (setup) UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end SetVSync

} // end Class GraphicsMenu
