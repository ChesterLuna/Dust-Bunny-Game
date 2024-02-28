using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] LayerMask _environmentLayer;
    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _gravity = 9.8f;
    [Tooltip("If true, the enemy will move back and forth, turning when they reach the edge of a platform or a wall")]
    [SerializeField] bool _patrol = true;
    [SerializeField] StartingDirection startingDirection = StartingDirection.Right;
    [SerializeField] bool _allowTurning = true;

    [SerializeField] MovementType _movementType = MovementType.velocity;

    [Header("Not Yet Implemented, Ignore for Now:")]
    [SerializeField] bool _seekPlayer = false;
    [Tooltip("The point from which the enemy will cast a ray to detect the player, if none is provided, the enemy will cast a ray from its own position.")]
    [SerializeField] Transform _raycastOriginPoint;
    [SerializeField] float _lineOfSightDistance = 5;
    [SerializeField] List<Transform> _patrolPoints = new List<Transform>();


    int _currentPatrolPoint = 0;


    Rigidbody2D _rb;
    Vector2 _newMovement;
    bool _isFacingRight = true;
    GameObject _player;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_raycastOriginPoint == null)
        {
            _raycastOriginPoint = transform;
        }
        if (startingDirection == StartingDirection.Left)
        {
            Turn(true);
        }
    } // end Awake

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    } // end Start

    void FixedUpdate()
    {
        if (_seekPlayer && CanSeePlayer())
        {
            SeekPlayer();
        }
        if (_patrol)
        {
            Patrol();
        }
        ApplyMovement();
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

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(_raycastOriginPoint.position, _player.transform.position);
        if (distanceToPlayer > _lineOfSightDistance) return false;

        Vector2 directionToPlayer = (_player.transform.position - _raycastOriginPoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(_raycastOriginPoint.position, directionToPlayer, _lineOfSightDistance, _environmentLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    } // end CanSeePlayer

    private void SeekPlayer()
    {
        // TODO: Pathfind to player
    } // end SeekPlayer

    private void Patrol()
    {
        if (_patrolPoints.Count > 0)
        {
            // TODO: Implement patrol points

            // Pathfind to _patrolPoints[_currentPatrolPoint]
            // If at _patrolPoints[_currentPatrolPoint], _currentPatrolPoint += 1
            // If _currentPatrolPoint >= _patrolPoints.Count, _currentPatrolPoint = 0
        }
        else
        { // Walk from edge to edge of platform
            Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
            _newMovement.x = direction.x;
        }
    } // end Patrol

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

    void OnDrawGizmos()
    {
        if (_seekPlayer)
        {
            Gizmos.color = Color.blue;
            if (_raycastOriginPoint == null)
            {
                _raycastOriginPoint = transform;
            }
            Gizmos.DrawWireSphere(_raycastOriginPoint.position, _lineOfSightDistance);
        }
    } // end OnDrawGizmos

    private enum StartingDirection
    {
        Right,
        Left
    } // end StartingDirection

    private enum MovementType
    {
        velocity,
        position
    } // end StartingDirection
} // end EnemyMovement

