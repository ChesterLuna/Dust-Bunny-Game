using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayOverlay : MonoBehaviour
{
    [SerializeField] GameObject _timerTextUI;

    void Start()
    {
        GameManager.instance.UpdateTimerText += OnUpdateTimerText;
        _timerTextUI.SetActive(GameManager.instance.ShowTimer);
    } // end Start

    void OnEnable()
    {
        _timerTextUI.SetActive(GameManager.instance.ShowTimer);
    } // end OnEnable

    public void OnUpdateTimerText()
    {
        if (_timerTextUI.activeSelf == false) return;
        float numSeconds = GameManager.instance.NumSeconds;
        // Convert seconds to clock format
        int minutes = (int)(numSeconds / 60);
        int seconds = (int)(numSeconds % 60);
        string clockFormat = string.Format("{0:00}:{1:00}", minutes, seconds);
        _timerTextUI.GetComponent<TextMeshProUGUI>().text = clockFormat;
    } // end UpdateScoreText
} // end SettingsMenu
