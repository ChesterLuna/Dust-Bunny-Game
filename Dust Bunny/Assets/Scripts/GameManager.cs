using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    public Vector3 CheckpointLocation;

    // Score Timer
    public int NumSeconds = 0;
    float _secondTimer;
    string _scoreTimerRunning = "Stopped";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    } // end Awake


    #region Timer
    public void StartGameTime()
    {
        UpdateTimerText();
        if (_scoreTimerRunning == "Stopped")
        {
            _scoreTimerRunning = "Running";
            StartCoroutine(GameTimeCoroutine());
        }
        else if (_scoreTimerRunning == "Paused")
        {
            _scoreTimerRunning = "Running";
        }
    } // end StartGameTime

    public void StopGameTime()
    {
        if (_scoreTimerRunning != "Stopped")
        {
            _scoreTimerRunning = "Stopped";
            StopCoroutine(GameTimeCoroutine());
        }
    } // end StopGameTime

    public void PauseGameTime()
    {
        if (_scoreTimerRunning == "Running")
        {
            _scoreTimerRunning = "Paused";
        }
    } // end StopGameTime

    private IEnumerator GameTimeCoroutine()
    {
        while (true)
        {
            if (_scoreTimerRunning == "Stopped")
            {
                break;
            }
            else if (_scoreTimerRunning == "Paused")
            {
                yield return null;
                continue;
            }
            _secondTimer = _secondTimer + Time.deltaTime;
            if (_secondTimer >= 1f)
            {
                _secondTimer = _secondTimer - 1f;
                NumSeconds += 1;
                UpdateTimerText();
            }
            yield return null;
        }
    } // end GameTimeCoroutine

    public void UpdateTimerText()
    {
        // Convert seconds to clock format
        int minutes = (int)(NumSeconds / 60);
        int seconds = (int)(NumSeconds % 60);
        string clockFormat = string.Format("{0:00}:{1:00}", minutes, seconds);
        GameObject.FindWithTag("TimerText").GetComponent<TextMeshProUGUI>().text = clockFormat;
    } // end UpdateScoreText
    #endregion Timer
} // end class GameManager
