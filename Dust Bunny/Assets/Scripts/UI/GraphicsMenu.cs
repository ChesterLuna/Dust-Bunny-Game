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
    [SerializeField] private List<string> _FPSOptions = new List<string> { "30", "60", "120", "144", "240", "Unlimited" };

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
        _FPSDropdown.AddOptions(_FPSOptions);
        int currentFPS = Application.targetFrameRate;
        int closestFPSIndex = 0;
        int smallestDifference = int.MaxValue;

        for (int i = 0; i < _FPSOptions.Count; i++)
        {
            int fpsOption;
            if (_FPSOptions[i] == "Unlimited")
            {
                fpsOption = int.MaxValue;
            }
            else
            {
                fpsOption = int.Parse(_FPSOptions[i]);
            }
            int difference = Math.Abs(currentFPS - (fpsOption - 1));
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closestFPSIndex = i;
            }
        }

        _FPSDropdown.value = closestFPSIndex;
        SetFPS(closestFPSIndex);
        _FPSDropdown.RefreshShownValue();
    } // end SetupFPSDropDown

    public void SetFPS(int fpsIndex)
    {
        int fps;
        if (_FPSOptions[fpsIndex] == "Unlimited")
        {
            fps = int.MaxValue;
        }
        else
        {
            fps = int.Parse(_FPSOptions[fpsIndex]);
        }
        Application.targetFrameRate = fps;
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
