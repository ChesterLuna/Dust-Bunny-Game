using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuOLD : MonoBehaviour
{
    [SerializeField] Toggle _displayTimerToggle;
    void OnEnable()
    {
        _displayTimerToggle.isOn = GameManager.Instance.ShowTimer;
        _displayTimerToggle.onValueChanged.AddListener((value) => GameManager.Instance.SetShowTimer(value));
    } // end OnEnable

    void OnDisable()
    {
        _displayTimerToggle.onValueChanged.RemoveAllListeners();
    } // end OnDisable
} // end SettingsMenu
