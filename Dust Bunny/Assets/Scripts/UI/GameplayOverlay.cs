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
        GameManager.Instance.UpdateTimerText += OnUpdateTimerText;
    } // end Start

    void OnEnable()
    {
        _timerTextUI.SetActive(GameManager.Instance.ShowTimer);
        if (GameManager.Instance.ShowTimer)
        {
            OnUpdateTimerText();
        }
        GameManager.Instance?.StartGameTime();
    } // end OnEnable

    void OnDisable()
    {
        _timerTextUI.SetActive(GameManager.Instance.ShowTimer);
        GameManager.Instance?.PauseGameTime();
    } // end OnDisable

    public void OnUpdateTimerText()
    {
        if (_timerTextUI == null) return;
        float numSeconds = GameManager.Instance.NumSeconds;
        // Convert seconds to clock format
        int minutes = (int)(numSeconds / 60);
        int seconds = (int)(numSeconds % 60);
        string clockFormat = string.Format("{0:00}:{1:00}", minutes, seconds);
        _timerTextUI.GetComponent<TextMeshProUGUI>().text = clockFormat;
    } // end UpdateScoreText
} // end SettingsMenu
