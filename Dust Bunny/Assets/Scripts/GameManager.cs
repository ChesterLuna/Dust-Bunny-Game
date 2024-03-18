using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    public Vector3? CheckpointLocation;
    // called zero
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

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    } // end OnEnable

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    } // end OnDisable

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ES3AutoSaveMgr.Current.Load();
        if (CheckpointLocation.HasValue)
        {
            FindObjectOfType<PlayerController>().gameObject.transform.position = CheckpointLocation.Value;
        }
    } // end OnSceneLoaded

    // called third
    private void Start()
    {
        StartGameTime();
    } // end Awake

    #region Timer
    // Score Timer
    public bool ShowTimer = false;
    public int NumSeconds = 0;
    float _secondTimer;
    string _scoreTimerRunning = "Stopped";
    public event Action UpdateTimerText;


    public void StartGameTime()
    {
        UpdateTimerText?.Invoke();
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
                UpdateTimerText?.Invoke();
            }
            yield return null;
        }
    } // end GameTimeCoroutine
    #endregion Timer
} // end class GameManager
