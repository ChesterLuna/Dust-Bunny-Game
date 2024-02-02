using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private int _index = 0;
    [SerializeField] private float _tolerance = 0.05f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _looped = false;
    [SerializeField] private bool _teleportStartOfLoop = false;
    [SerializeField] private bool _ascending = true;



    private void FixedUpdate()
    {
        var target = _waypoints[_index].position;
        transform.position = Vector2.MoveTowards(transform.position, target, _speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) <= _tolerance)
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
                        transform.position = _waypoints[_index].position;
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

} // end class MovingPlatform
