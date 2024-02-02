using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A timer that counts down from a specified time.
/// </summary>
public class Timer : MonoBehaviour
{
    private float _timeRemaining;
    private bool _expired = false;
    private bool _pasued = false;

    /// <summary>
    /// Updates the timer by decreasing the remaining time if not paused or expired.
    /// </summary>
    void Update()
    {
        if (_pasued == false && _expired == false)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
            }
            else
            {
                _timeRemaining = 0;
                _expired = true;
            }
        }
    }

    /// <summary>
    /// Starts the timer with the specified time.
    /// </summary>
    /// <param name="time">The time in seconds.</param>
    public void StartTimer(float time = 10)
    {
        _pasued = false;
        _expired = false;
        _timeRemaining = time;
    }

    /// <summary>
    /// Checks if the timer is not expired and has time left.
    /// </summary>
    /// <returns>True if the timer is valid, false otherwise.</returns>
    public bool IsValid()
    {
        return _timeRemaining > 0 && !_expired;
    }

    /// <summary>
    /// Checks if the timer is actively running.
    /// </summary>
    /// <returns>True if the timer is running, false otherwise.</returns>
    public bool IsRunning()
    {
        return IsValid() && !_pasued;
    }

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    public void Pause()
    {
        _pasued = true;
    }

    /// <summary>
    /// Resumes the timer.
    /// </summary>
    public void Resume()
    {
        _pasued = false;
    }

    /// <summary>
    /// Completly stops/clears the timer.
    /// </summary>
    public void Stop()
    {
        _pasued = true;
        _expired = true;
        _timeRemaining = 0;
    }

    /// <summary>
    /// Returns the remaining time.
    /// </summary>
    /// <returns>The remaining time.</returns>
    public float GetTimeRemaining()
    {
        return _timeRemaining;
    }

    /// <summary>
    /// Sets the remaining time for the timer.
    /// </summary>
    /// <param name="time">The time remaining.</param>
    public void SetTimeRemaining(float time)
    {
        _timeRemaining = time;
    }

    /// <summary>
    /// Adds the specified amount of time to the remaining time.
    /// </summary>
    /// <param name="time">The amount of time to add.</param>
    public void AddTime(float time)
    {
        _timeRemaining += time;
    }

    /// <summary>
    /// Subtracts the specified amount of time from the remaining time.
    /// </summary>
    /// <param name="time">The amount of time to subtract.</param>
    public void SubtractTime(float time)
    {
        _timeRemaining -= time;
    }
}
