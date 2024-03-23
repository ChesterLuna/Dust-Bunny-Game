using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PlatformBase
{
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField, Range(0, 100)] private float _duration = 2;
    [SerializeField, Range(0, 20)] private float _endPauseDuration = 0.2f;
    [SerializeField] private bool _loop;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private bool _smoothPath;
    [HideInInspector] private Vector2[] _runtimePoints;

    private float _time, _currentPauseTime;
    private bool _ascending = true;
    private IPatrolPath _patrolPath;
    private const int DEBUG_RESOLUTION = 100;

    protected override void Awake()
    {
        base.Awake();

        CreateRuntimePoints();
    } // end Awake

    public override void OnValidate()
    {
        base.OnValidate();
        CreateRuntimePoints();
    } // end OnValidate

    protected override Vector2 Evaluate(float delta)
    {
        _currentPauseTime += delta;

        if (_currentPauseTime < _endPauseDuration)
        {
            Rb.gravityScale = 0;
            return Rb.position;
        }

        Rb.gravityScale = 1;

        if (_ascending || _loop) _time += delta / _duration;
        else _time -= delta / _duration;

        _time = Mathf.Clamp(_time, 0, 1);
        var curveTime = _curve.Evaluate(_time);

        if (_time is >= 1 or <= 0)
        {
            if (_loop) _time = 0;
            _ascending = !_ascending;
            _currentPauseTime = 0;
        }

        return _patrolPath?.GetPointAtDistance(curveTime) ?? Vector2.zero;
    } // end Evaluate

    private void CreateRuntimePoints()
    {
        if (_patrolPoints == null || _patrolPoints.Length < 2)
        {
            _patrolPath = null;
            return; // Show editor warning
        }

        _runtimePoints = new Vector2[_patrolPoints.Length];

        for (var i = 0; i < _patrolPoints.Length; i++)
        {
            _runtimePoints[i] = _patrolPoints[i].position;
        }

        _patrolPath = _smoothPath ? new SmoothPatrol(_runtimePoints) : new LinearPatrol(_runtimePoints);
    } // end CreateRuntimePoints

    private void OnDrawGizmosSelected()
    {
        if (_patrolPath == null) return;
        Gizmos.color = Color.magenta;

        for (var i = 0; i < DEBUG_RESOLUTION; i++)
        {
            var t1 = i / (float)DEBUG_RESOLUTION;
            var t2 = (i + 1) / (float)DEBUG_RESOLUTION;

            var point1 = _patrolPath.GetPoint(t1);
            var point2 = _patrolPath.GetPoint(t2);

            Gizmos.DrawLine(point1, point2);
        }
    } // end OnDrawGizmosSelected
} // end class PatrolPlatform