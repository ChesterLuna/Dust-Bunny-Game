using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : RigidBodyRideable, ISwitchable
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private int _index = 0;
    [SerializeField] private float _tolerance = 0.05f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _looped = false;
    [SerializeField] private bool _teleportStartOfLoop = false;
    [SerializeField] private bool _ascending = true;
    [SerializeField] private bool _moving = true;

    private void FixedUpdate()
    {
        if (!_moving) return;

        var target = _waypoints[_index].position;
        Vector3 direction = (target - transform.position).normalized;
        transform.position = transform.position + direction * _speed * Time.fixedDeltaTime;
        // MoveWithRiders();
        // _rb.MovePosition(transform.position + direction * _speed * Time.deltaTime);
        if (Vector2.Distance(_rb.position, target) <= _tolerance)
        {
            // /Debug.Log(this.name + ": " + _index);
            _index = _ascending ? _index + 1 : _index - 1;
            if (_index >= _waypoints.Length)
            {
                if (_looped)
                {
                    _index = 0;
                    if (_teleportStartOfLoop)
                    {
                        _rb.position = _waypoints[_index].position;
                    }
                }
                else
                {
                    _ascending = false;
                    _index--;
                }
            }
            else if (_index < 0)
            {
                _ascending = true;
                _index = 1;
            }
        }
    } // end FixedUpdate

    public void Disable()
    {
        _moving = false;
    }

    public void Enable()
    {
        _moving = true;
    }


    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        var previous = (Vector2)transform.position;
        for (var i = 0; i < _waypoints.Length; i++)
        {
            var p = (Vector2)_waypoints[i].position;
            Gizmos.DrawWireSphere(p, 0.2f);
            Gizmos.DrawLine(previous, p);

            previous = p;

            if (_looped && i == _waypoints.Length - 1) Gizmos.DrawLine(p, (Vector2)_waypoints[0].position);
        }
    } // end OnDrawGizmosSelected
} // end class MovingPlatform
