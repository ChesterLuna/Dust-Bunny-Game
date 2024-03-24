using UnityEngine;

public abstract class PlatformBase : MonoBehaviour, IPhysicsObject, IPhysicsMover
{
    public bool UsesBounding => _boundingEffector != null;
    public bool RequireGrounding => _requireGrounding;
    public Vector2 FramePositionDelta { get; private set; }
    public Vector2 FramePosition => Rb.position;
    public Vector2 Velocity => Rb.velocity;
    public Vector2 TakeOffVelocity => _useTakeOffVelocity ? Velocity : Vector2.zero;

    [HideInInspector] protected Rigidbody2D Rb;
    [SerializeField] private bool _requireGrounding;
    [SerializeField] private BoxCollider2D _boundingEffector;
    [SerializeField] private bool _useTakeOffVelocity;

    protected float Time;

    public virtual void OnValidate()
    {
        if (_boundingEffector) _boundingEffector.isTrigger = true;
    }

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        PhysicsSimulator.Instance.AddPlatform(this);
    }

    public void TickFixedUpdate(float delta)
    {
        var newPos = Evaluate(delta);

        // Position
        var positionDifference = newPos - Rb.position;
        if (positionDifference.sqrMagnitude > 0)
        {
            FramePositionDelta = positionDifference;
            Rb.velocity = FramePositionDelta / delta;
        }
        else
        {
            FramePositionDelta = Vector3.zero;
            Rb.velocity = Vector3.zero;
        }
    }

    public void TickUpdate(float delta, float time)
    {
        Time = time;
    }

    protected abstract Vector2 Evaluate(float delta);
}
