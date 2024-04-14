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

    [SerializeField] public GameplayState CurrentGameState = GameplayState.MainMeu;
    [SerializeField] public Vector3? CheckpointLocation;
    public float CheckpointDustLevel = -1;


    // called zero
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ShowTimer = PlayerPrefs.GetInt("ShowTimer", 0) == 1;
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
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            Debug.Log("Deleting Save File");
            ES3.DeleteFile("SaveFile.es3", new ES3Settings(ES3.Location.Cache));
            ES3.DeleteFile("SaveFile.es3", new ES3Settings(ES3.Location.File));
            ES3.DeleteFile("SaveFile.es3", new ES3Settings(ES3.Location.PlayerPrefs));

            ResetGameTime();
            CheckpointLocation = null;
            CheckpointDustLevel = -1;
        }
        else if (ES3AutoSaveMgr.Current != null)
        {
            ES3AutoSaveMgr.Current.Load();
        }
    } // end OnSceneLoaded

    // called third
    private void Start()
    {

    } // end Awake



    #region Timer
    // Score Timer
    public bool ShowTimer;
    public int NumSeconds = 0;
    float _secondTimer = 0;
    string _scoreTimerRunning = "Stopped";
    public event Action UpdateTimerText;


    public void StartGameTime()
    {
        UpdateTimerText?.Invoke();
        if (SceneManager.GetActiveScene().name == "Good Ending") return;

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

    public void ResetGameTime()
    {
        StopCoroutine(GameTimeCoroutine());
        NumSeconds = 0;
        _secondTimer = 0;
    } // end ResetGameTime

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

    public void SetShowTimer(bool value)
    {
        ShowTimer = value;
        PlayerPrefs.SetInt("ShowTimer", value ? 1 : 0);
    } // end SetMouseForDash
    #endregion Timer
} // end class GameManager

public enum GameplayState
{
    MainMeu,
    Paused,
    Playing,
    Dialogue,
    Dead
} // end enum GameplayState