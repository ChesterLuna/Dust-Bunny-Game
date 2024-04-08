using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Toggle _displayTimerToggle;
    void OnEnable()
    {
        _displayTimerToggle.isOn = GameManager.instance.ShowTimer;
        _displayTimerToggle.onValueChanged.AddListener((value) => GameManager.instance.SetShowTimer(value));
    } // end OnEnable

    void OnDisable()
    {
        _displayTimerToggle.onValueChanged.RemoveAllListeners();
    } // end OnDisable
} // end SettingsMenu
