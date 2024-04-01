using System;
using System.Collections.Generic;
using UnityEngine;

public class SwifferPlatform : PlatformBase
{
    [Header("Movement Variables")]
    PlayerController _player;
    Vector3 _previousPosition;
    Vector3 _currentPosition;
    [SerializeField] float _minMoveDistance = 0.01f;
    [SerializeField] bool _isFacingRight = true;
    [SerializeField] LayerMask _environmentLayer;
    [SerializeField] LayerMask _playerLayer;

    [SerializeField] float _moveSpeed = 0.1f;

    [Header("Seek Settings")]
    [SerializeField] bool _seekPlayer = false;
    [Tooltip("The point from which the enemy will cast a ray to detect the player, if none is provided, the enemy will cast a ray from its own position.")]
    [SerializeField] Transform _raycastOriginPoint;
    [SerializeField] float _lineOfSightDistance = 5f;
    [SerializeField] float _minXDistance = 0.1f;

    [Header("Patrol Settings")]
    [Tooltip("The points the enemy will patrol between, if none are provided, the enemy will wander.")]
    [SerializeField] Transform[] _patrolPoints;
    List<bool> reachablePoints = new List<bool>();
    [SerializeField] float _patrolThreshold = 0.2f;
    private int _patrolPointAdder = 1;
    int _currentPatrolPointIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        if (_raycastOriginPoint == null)
        {
            _raycastOriginPoint = transform;
        }

        if (_patrolPoints.Length > 0)
        {
            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                reachablePoints.Add(true);
            }
        }
    }

    public override void OnValidate()
    {
        base.OnValidate();
        if (_raycastOriginPoint == null)
        {
            _raycastOriginPoint = transform;
        }
    }

    void Start()
    {
        _player = GameObject.FindObjectOfType<PlayerController>();
    } // end Start

    protected override Vector2 Evaluate(float delta)
    {
        _previousPosition = _currentPosition;
        _currentPosition = transform.position;

        float targetDirection;
        if (_seekPlayer && CanSeePlayer())
        { // Seek Player
            Vector2 direction = _player.State.Position - (Vector2)transform.position;
            targetDirection = Mathf.Sign(direction.x);
            Debug.Log(direction.x);
            if (Mathf.Abs(direction.x) < _minXDistance)
            {
                return new Vector2(transform.position.x, transform.position.y);
            }
            else
            {
                var newPos = new Vector2(transform.position.x + (targetDirection * _moveSpeed), transform.position.y);
                // Debug.Log(newPos);
                return newPos;
            }
            // bool tempIsFacingRight = targetDirection == 1; // Set _isFacingRight to true if targetDirection is +1 (right) and false if -1 (left)
            // if (_isFacingRight != tempIsFacingRight)
            // {
            //     Turn();
            // }
        }
        else
        { // Patrol
            Debug.Log("Patrolling");
            // Debug.Log(HasMoved() + " " + _patrolThreshold + " " + Math.Abs(transform.position.x - _patrolPoints[_currentPatrolPointIndex].position.x) + " " + _currentPatrolPointIndex);
            if (!HasMoved() || _patrolThreshold > Math.Abs(transform.position.x - _patrolPoints[_currentPatrolPointIndex].position.x))
            {
                IncrementPatrolPoints();
            }
            Transform _currentPatrolPoint = _patrolPoints[_currentPatrolPointIndex];
            Vector2 direction = _currentPatrolPoint.position - transform.position;
            targetDirection = Mathf.Sign(direction.x);
            // bool tempIsFacingRight = targetDirection == 1; // Set _isFacingRight to true if targetDirection is +1 (right) and false if -1 (left)
            // if (_isFacingRight != tempIsFacingRight)
            // {
            //     Turn();
            // }
            // _newMovement.x = targetDirection;
            // return _newMovement * _moveSpeed;
            var newPos = new Vector2(transform.position.x + (targetDirection * _moveSpeed), transform.position.y);
            // Debug.Log(newPos);
            return newPos;
        }


    }
    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(_raycastOriginPoint.position, _player.State.Position);
        if (distanceToPlayer > _lineOfSightDistance) return false;

        Vector2 directionToPlayer = (_player.GetColliderPosition() - (Vector2)_raycastOriginPoint.position).normalized;

        bool tempStart = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(_raycastOriginPoint.position, directionToPlayer, _lineOfSightDistance, _playerLayer + _environmentLayer);
        Physics2D.queriesStartInColliders = tempStart;
        Debug.DrawRay(_raycastOriginPoint.position, directionToPlayer * _lineOfSightDistance, Color.red);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            if (hit.collider != null) Debug.Log(hit.collider.name);
            return false;
        }
    } // end CanSeePlayer

    bool HasMoved()
    {
        return Vector3.Distance(_previousPosition, _currentPosition) > _minMoveDistance;
    } // end HasMoved

    private void IncrementPatrolPoints()
    {
        if (_patrolPoints.Length == 0) return;
        _currentPatrolPointIndex += _patrolPointAdder;
        if (_currentPatrolPointIndex >= _patrolPoints.Length)
        {

            _patrolPointAdder = -1;
            _currentPatrolPointIndex = _patrolPoints.Length - 1;
        }
        else if (_currentPatrolPointIndex < 0)
        {
            _patrolPointAdder = 1;
            _currentPatrolPointIndex = 0;
        }
    } // end IncrementPatrolPoints

    public void Turn()
    {
        // Placeholder for now
        _isFacingRight = !_isFacingRight;
    } // end Turn
}