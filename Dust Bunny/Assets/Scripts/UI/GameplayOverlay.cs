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
    } // end Start

    void OnEnable()
    {
        if(GameManager.instance == null) return;
        
        _timerTextUI.SetActive(GameManager.instance.ShowTimer);
        if (GameManager.instance.ShowTimer)
        {
            OnUpdateTimerText();
        }
        GameManager.instance?.StartGameTime();
    } // end OnEnable

    void OnDisable()
    {
        _timerTextUI.SetActive(GameManager.instance.ShowTimer);
        GameManager.instance?.PauseGameTime();
    } // end OnDisable

    public void OnUpdateTimerText()
    {
        if (_timerTextUI == null) return;
        float numSeconds = GameManager.instance.NumSeconds;
        // Convert seconds to clock format
        int minutes = (int)(numSeconds / 60);
        int seconds = (int)(numSeconds % 60);
        string clockFormat = string.Format("{0:00}:{1:00}", minutes, seconds);
        _timerTextUI.GetComponent<TextMeshProUGUI>().text = clockFormat;
    } // end UpdateScoreText
} // end SettingsMenu
