using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class EnemyMovement : MonoBehaviour
{

    [Header("Movement Variables")]
    GameObject _player;
    Rigidbody2D _rb;
    Vector2 _newMovement;
    Vector3 _previousPosition;
    Vector3 _currentPosition;
    [SerializeField] float _minMoveDistance = 0.01f;
    [SerializeField] bool _isFacingRight = true;
    [SerializeField] LayerMask _environmentLayer;
    [SerializeField] LayerMask _playerLayer;

    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _gravity = 9.8f;
    [SerializeField] bool _allowTurning = true;
    [SerializeField] MovementType _movementType = MovementType.velocity;

    [Header("Seek Settings")]
    [SerializeField] bool _seekPlayer = false;
    [Tooltip("The point from which the enemy will cast a ray to detect the player, if none is provided, the enemy will cast a ray from its own position.")]
    [SerializeField] Transform _raycastOriginPoint;
    [SerializeField] float _lineOfSightDistance = 5f;

    [Header("Wander Settings")]
    [SerializeField] bool _wander = true;
    bool waitExtraFrame = false;

    [Header("Patrol Settings")]
    [SerializeField] bool _patrol = true;
    [Tooltip("The points the enemy will patrol between, if none are provided, the enemy will wander.")]
    [SerializeField] Transform[] _patrolPoints;
    List<bool> reachablePoints = new List<bool>();
    [SerializeField] bool _loopPoints = false;
    [SerializeField] float _patrolThreshold = 0.2f;
    private int _patrolPointAdder = 1;
    int _currentPatrolPointIndex = 0;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_raycastOriginPoint == null)
        {
            _raycastOriginPoint = transform;
        }
        if (!_isFacingRight)
        {
            _isFacingRight = true;
            Turn(true);
        }

        if (_patrolPoints.Length > 0)
        {
            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                reachablePoints.Add(true);
            }
        }
    } // end Awake

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    } // end Start

    void FixedUpdate()
    {
        Print("Seek: " + _seekPlayer + " CanSeePlayer: " + CanSeePlayer() + " Wander: " + _wander + " CantReachPatrolPoints: " + CantReachPatrolPoints() + " Patrol: " + _patrol);
        if (_seekPlayer && CanSeePlayer())
        {
            SeekPlayer();
        }
        else if (_wander && CantReachPatrolPoints())
        {
            Wander();
        }
        else if (_patrol)
        {
            Patrol();
        }
        _previousPosition = _currentPosition;
        ApplyMovement();
        _currentPosition = transform.position;
    } // end FixedUpdate

    void ApplyMovement()
    {
        // Set proper speed & grivity
        _newMovement = new Vector2(_newMovement.x * _moveSpeed, _newMovement.y - _gravity);

        // Apply the movement
        if (_movementType == MovementType.velocity)
        {
            _rb.velocity = new Vector2(_newMovement.x * _moveSpeed, _newMovement.y - _gravity);
        }
        else if (_movementType == MovementType.position)
        {
            transform.position = transform.position + (Vector3)_newMovement * Time.fixedDeltaTime;
        }
    } // end ApplyMovement

    bool HasMoved()
    {
        return Vector3.Distance(_previousPosition, _currentPosition) > _minMoveDistance;
    } // end HasMoved


    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(_raycastOriginPoint.position, _player.transform.position);
        if (distanceToPlayer > _lineOfSightDistance) return false;

        Vector2 directionToPlayer = (_player.transform.position - _raycastOriginPoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(_raycastOriginPoint.position, directionToPlayer, _lineOfSightDistance, _playerLayer + _environmentLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    } // end CanSeePlayer

    private float FacePlayer()
    {
        Vector2 direction = _player.transform.position - transform.position;
        float targetDirection = Mathf.Sign(direction.x);
        bool tempIsFacingRight = targetDirection == 1; // Set _isFacingRight to true if targetDirection is +1 (right) and false if -1 (left)
        if (_isFacingRight != tempIsFacingRight)
        {
            Turn();
        }
        return targetDirection;
    } // end FacePlayer


    private void SeekPlayer()
    {
        _newMovement.x = FacePlayer();
    } // end SeekPlayer

    private bool CantReachPatrolPoints()
    {
        return _patrolPoints.Length == 0 || !reachablePoints.Contains(true);
    } // end CantReachPatrolPoints

    private void Patrol()
    {
        if (_patrolPoints.Length == 0) return;
        if (!HasMoved())
        {
            reachablePoints[_currentPatrolPointIndex] = false;
        }
        if (!HasMoved() || _patrolThreshold > Math.Abs(transform.position.x - _patrolPoints[_currentPatrolPointIndex].position.x))
        {
            _currentPatrolPointIndex += _patrolPointAdder;
            if (_currentPatrolPointIndex >= _patrolPoints.Length)
            {
                if (_loopPoints)
                {
                    _currentPatrolPointIndex = 0;
                }
                else
                {
                    _patrolPointAdder = -1;
                    _currentPatrolPointIndex = _patrolPoints.Length - 1;
                }
            }
            else if (_currentPatrolPointIndex < 0)
            {
                _patrolPointAdder = 1;
                _currentPatrolPointIndex = 0;
            }
        }
        Transform _currentPatrolPoint = _patrolPoints[_currentPatrolPointIndex];
        Vector2 direction = _currentPatrolPoint.position - transform.position;
        float targetDirection = Mathf.Sign(direction.x);
        bool tempIsFacingRight = targetDirection == 1; // Set _isFacingRight to true if targetDirection is +1 (right) and false if -1 (left)
        if (_isFacingRight != tempIsFacingRight)
        {
            Turn();
        }
        _newMovement.x = targetDirection;
    } // end Patrol

    private void Wander()
    {
        if (!HasMoved() && waitExtraFrame)
        {
            Turn();
            waitExtraFrame = false;
        }
        else if (!HasMoved())
        {
            waitExtraFrame = true;
        }
        // Walk from edge to edge of platform
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        _newMovement.x = direction.x;
    } // end Wander

    public void Turn(bool overwrite = false)
    {
        if (!_allowTurning && !overwrite) return;
        Vector3 rotator;
        if (_isFacingRight)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        }
        transform.rotation = Quaternion.Euler(rotator);
        _isFacingRight = !_isFacingRight;
    } // end Turn

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        var previous = (Vector2)transform.position;
        for (var i = 0; i < _patrolPoints.Length; i++)
        {
            var p = (Vector2)_patrolPoints[i].position;
            Gizmos.DrawWireSphere(p, 0.2f);
            Gizmos.DrawLine(previous, p);

            previous = p;

            if (_loopPoints && i == _patrolPoints.Length - 1) Gizmos.DrawLine(p, (Vector2)_patrolPoints[0].position);
        }

        if (_seekPlayer)
        {
            Gizmos.color = Color.blue;
            if (_raycastOriginPoint == null)
            {
                _raycastOriginPoint = transform;
            }
            Gizmos.DrawWireSphere(_raycastOriginPoint.position, _lineOfSightDistance);
        }
    } // end OnDrawGizmosSelected

    private enum MovementType
    {
        velocity,
        position
    } // end MovementType

    #region Debugging
    [Header("Debug")]
    [SerializeField] bool PrintDebug = false;

    void Print(string message)
    {
        if (PrintDebug)
        {
            Debug.Log(message);
        }
    } // end Print

    void Print(bool message)
    {
        if (PrintDebug)
        {
            Debug.Log(message);
        }
    } // end Print

    void Print(int message)
    {
        if (PrintDebug)
        {
            Debug.Log(message);
        }
    } // end Print
    void Print(float message)
    {
        if (PrintDebug)
        {
            Debug.Log(message);
        }
    } // end Print
    #endregion
} // end EnemyMovement

