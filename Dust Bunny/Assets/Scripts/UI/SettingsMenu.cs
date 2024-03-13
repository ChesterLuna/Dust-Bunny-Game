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
        // bool temp = GameManager.instance.ShowTimer;
        // if (GameManager.instance.ShowTimer) _displayTimerToggle.Select();
        // GameManager.instance.ShowTimer = temp;
        _displayTimerToggle.isOn = GameManager.instance.ShowTimer;
    } // end OnEnable
} // end SettingsMenu
