using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Pathing")]
    // Custom Pathing Variables
    [SerializeField] protected LayerMask EnviromentLayers;
    [SerializeField] protected float LOSActivateDistance = 30f;
    [SerializeField] protected bool PreventPatrolLedgeWalkOff;

    protected bool HasLineOfSight;
    protected float SightTimer;
    protected float LineOfSightRegainTime = 5f;
    protected float WallHitCheckDistance = 0.7f;

    [Header("GENERAL")]
    protected float GrounderDistance = 0.1f;
    protected Rigidbody2D EnemyRb;
    protected CapsuleCollider2D EnemyCollider;
    protected Vector2 Speed;
    protected Vector2 CurrentExternalVelocity;


    // Enemy Control and Movement
    [SerializeField] protected bool Stationary;
    [SerializeField] protected int DirectionFacing = 1;
    [SerializeField] protected bool UseGravity = false;
    [SerializeField] protected float MoveSpeed = 5;
    [SerializeField] protected float FallSpeed = 70f;
    [SerializeField] protected float MaxFallSpeed = 40;

    // World Collision Detection
    protected readonly RaycastHit2D[] GroundHits = new RaycastHit2D[1];
    protected readonly RaycastHit2D[] CeilingHits = new RaycastHit2D[1];
    protected bool Grounded;
    protected bool HitCeiling;


    protected void Start()
    {
        EnemyRb = GetComponent<Rigidbody2D>();
        EnemyCollider = GetComponent<CapsuleCollider2D>();
        transform.localScale = new Vector3(DirectionFacing * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    } // end Start

    protected void FixedUpdate()
    {
        CheckCollisions();
        CalculateMovement();
    } // end Update

    // General Pathfinding
    protected virtual void CalculateMovement()
    {
        if (!Stationary)
        {
            SimplePatrol();
        }
        if (UseGravity)
        {
            CalculateGravity();
        }
        ApplyVelocity();
    } // end CalculateMovement

    protected void SimplePatrol()
    {
        RaycastHit2D wallHitCheck = Physics2D.Raycast(transform.position, new Vector2(DirectionFacing, 0), WallHitCheckDistance, EnviromentLayers);
        Debug.DrawRay(transform.position, new Vector2(DirectionFacing, 0) * WallHitCheckDistance, Color.red);
        if (PreventPatrolLedgeWalkOff)
        {
            RaycastHit2D groundCheck = Physics2D.Raycast(transform.position + DirectionFacing * new Vector3(EnemyCollider.size.x, 0, 0), Vector2.down, EnemyCollider.size.y / 2 + 0.1f, EnviromentLayers);
            Debug.DrawRay(transform.position + DirectionFacing * new Vector3(EnemyCollider.size.x, 0, 0), Vector2.down * (EnemyCollider.size.y / 2 + 0.1f), Color.red);
            if (!groundCheck && Speed.y == 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                DirectionFacing = (int)Mathf.Sign(transform.localScale.x) * 1;
            }
        }
        if (wallHitCheck)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            DirectionFacing = (int)Mathf.Sign(transform.localScale.x) * 1;
        }
        Speed.x = MoveSpeed * DirectionFacing;
    } // end SimplePatrol


    #region World Interactions
    protected virtual void ApplyVelocity()
    {
        EnemyRb.velocity = Speed + CurrentExternalVelocity;
    } // end ApplyVelocity

    public virtual void ApplyVelocity(Vector2 vel, EnemyForce forceType)
    {
        if (forceType == EnemyForce.Burst)
        {
            Speed += vel;
        }
        else
        {
            CurrentExternalVelocity += vel;
        }
    } // end ApplyVelocity

    public enum EnemyForce
    {
        /// <summary>
        /// Added directly to the Enemy's movement speed, to be controlled by the standard deceleration
        /// </summary>
        Burst,

        /// <summary>
        /// An additive force handled by the decay system
        /// </summary>
        Decay
    } // end enum EnemyForce

    protected void FaceTarget(Transform target)
    {
        DirectionFacing = 1 * (int)Mathf.Sign((target.position - transform.position).x);
        transform.localScale = new Vector3(DirectionFacing * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    } // end FaceTarget

    protected bool LineOfSight(Transform target)
    {
        Vector3 targetDir = target.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDir, LOSActivateDistance, EnviromentLayers);
        if (hit.collider == null)
        {
            if (HasLineOfSight)
            {
                SightTimer = Time.time + LineOfSightRegainTime;
            }
            //Debug.Log(SightTimer);
            return false;
        }
        else if (hit.collider.transform == target)
        {
            //Debug.Log("We hit the target! " + hit.transform.name);
            Debug.DrawRay(transform.position, targetDir, Color.blue);
            return true;
        }
        else
        {
            //Debug.Log("We did not hit target " + hit.transform.name);
            if (HasLineOfSight)
            {
                SightTimer = Time.time + LineOfSightRegainTime;
            }
            return false;
        }
    } // end LineOfSight

    protected void CalculateGravity()
    {
        if (Grounded)
        {
            // Move out of the ground if in it
            if (Speed.y < 0)
            {
                Speed.y = 0;
            }
        }
        else
        {
            Speed.y -= FallSpeed * Time.fixedDeltaTime;
            if (Speed.y < -MaxFallSpeed)
            {
                Speed.y = -MaxFallSpeed;
            }
        }
    } // end CalculateGravity

    protected virtual void CheckCollisions()
    {
        Vector2 offset = (Vector2)transform.position + EnemyCollider.offset;

        int groundHitCount = Physics2D.CapsuleCastNonAlloc(offset, EnemyCollider.size, EnemyCollider.direction, 0, Vector2.down, GroundHits, GrounderDistance, EnviromentLayers);
        int ceilingHitCount = Physics2D.CapsuleCastNonAlloc(offset, EnemyCollider.size, EnemyCollider.direction, 0, Vector2.up, CeilingHits, GrounderDistance, EnviromentLayers);

        if (ceilingHitCount > 0)
        {
            HitCeiling = true;
            if (Speed.y > 0)
            {
                Speed.y = 0;
            }
        }
        else
        {
            HitCeiling = false;
        }

        if (!Grounded && groundHitCount > 0)
        {
            Grounded = true;
        }
        else if (Grounded && groundHitCount == 0)
        {
            Grounded = false;
        }
    } // end CheckCollisions
    #endregion
} // end EnemyBase Class
