using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class RidableEnemy : RigidBodyRideable
{
    [SerializeField] LayerMask _environmentLayer;
    [SerializeField] float _moveSpeed = 2f;
    [Tooltip("If true, the enemy will move back and forth, turning when they reach the edge of a platform or a wall")]
    [SerializeField] bool _patrol = true;
    [SerializeField] StartingDirection startingDirection = StartingDirection.Right;
    [SerializeField] bool _allowTurning = true;

    Vector2 _newMovement;
    bool _isFacingRight = true;
    GameObject _player;

    protected override void Awake()
    {
        base.Awake();
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
        if (_patrol)
        {
            Patrol();
        }
        ApplyMovement();
    } // end FixedUpdate

    void ApplyMovement()
    {
        // Set proper speed & grivity
        _newMovement = new Vector2(_newMovement.x * _moveSpeed, _newMovement.y);

        // Apply the movement
        transform.position = transform.position + (Vector3)_newMovement * Time.fixedDeltaTime;
        MoveWithRiders();
    } // end ApplyMovement


    private void Patrol()
    {
        // Walk from edge to edge of platform
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        _newMovement.x = direction.x;

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
        TurnWithRiders();
    } // end Turn

    protected void TurnWithRiders()
    {
        foreach (PlayerController rider in _riders)
        {
            rider.IsFacingRight = !rider.IsFacingRight;
        }
    } // TurnWithRiders

    private enum StartingDirection
    {
        Right,
        Left
    } // end StartingDirection
} // end RidableEnemy

