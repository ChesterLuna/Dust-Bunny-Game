using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPlatform : PlatformBase
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
    }

    public override void OnValidate()
    {
        base.OnValidate();
        CreateRuntimePoints();
    }

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
    }

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
    }

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
        }//
    }
}

public interface IPatrolPath
{
    Vector2 GetPointAtDistance(float t);
    Vector2 GetPoint(float t);
}

public class LinearPatrol : IPatrolPath
{
    private readonly Vector2[] _points;
    private readonly float[] _segmentLengths;
    private readonly float _totalLength;

    public LinearPatrol(Vector2[] points)
    {
        _points = points;

        _segmentLengths = new float[_points.Length - 1];
        _totalLength = 0;
        for (var i = 0; i < _points.Length - 1; i++)
        {
            _segmentLengths[i] = Vector2.Distance(_points[i], _points[i + 1]);
            _totalLength += _segmentLengths[i];
        }
    }

    public Vector2 GetPointAtDistance(float t)
    {
        if (_points.Length == 0) return Vector2.zero;
        var targetDistance = Mathf.Clamp01(t) * _totalLength;
        var currentDistance = 0f;

        // Find the segment that the target distance falls into
        var segmentIndex = 0;
        for (; segmentIndex < _segmentLengths.Length; segmentIndex++)
        {
            if (currentDistance + _segmentLengths[segmentIndex] >= targetDistance) break;

            currentDistance += _segmentLengths[segmentIndex];
        }

        // If we didn't find a segment, return the last point
        if (segmentIndex == _segmentLengths.Length) return _points[^1];

        // Calculate how far into the segment we are
        var remainingDistance = targetDistance - currentDistance;
        var segmentFraction = remainingDistance / _segmentLengths[segmentIndex];

        // Interpolate between the two points in the segment
        return Vector2.Lerp(_points[segmentIndex], _points[segmentIndex + 1], segmentFraction);
    }

    public Vector2 GetPoint(float t)
    {
        var numSegments = _points.Length - 1;

        t = Mathf.Clamp01(t) * numSegments;
        var i = Mathf.FloorToInt(t);
        i = Mathf.Clamp(i, 0, numSegments - 1);

        var localT = t - i;
        localT = Mathf.Clamp(localT, 0.0f, 1.0f - Mathf.Epsilon);
        return Vector2.Lerp(_points[i], _points[i + 1], localT);
    }
}

public class SmoothPatrol : IPatrolPath
{
    private readonly Vector2[] _points;
    private readonly Vector2[] _tempPoints;
    private readonly List<float> _arcLengths;
    private readonly float _totalArcLength;

    public SmoothPatrol(Vector2[] points, int resolution = 1000)
    {
        _points = points;

        _tempPoints = new Vector2[_points.Length];

        _arcLengths = new List<float>();
        _totalArcLength = 0f;

        var lastPoint = _points[0];
        for (var i = 1; i <= resolution; i++)
        {
            var t = i / (float)resolution;
            var point = GetPoint(t);
            var segmentLength = Vector2.Distance(lastPoint, point);
            _totalArcLength += segmentLength;
            _arcLengths.Add(_totalArcLength);
            lastPoint = point;
        }
    }

    public Vector2 GetPointAtDistance(float t)
    {
        if (_points.Length == 0) return Vector2.zero;

        var distance = t * _totalArcLength;

        var index = _arcLengths.FindIndex(x => x >= distance);
        if (index == -1) return _points[^1];

        var tt = index / (float)_arcLengths.Count;
        return GetPoint(tt);
    }

    public Vector2 GetPoint(float t)
    {
        var n = _points.Length - 1;
        Array.Copy(_points, _tempPoints, _points.Length);

        for (var r = 1; r <= n; r++)
        {
            for (var i = 0; i <= n - r; i++)
            {
                _tempPoints[i] = Vector2.Lerp(_tempPoints[i], _tempPoints[i + 1], t);
            }
        }

        return _tempPoints[0];
    }
}
