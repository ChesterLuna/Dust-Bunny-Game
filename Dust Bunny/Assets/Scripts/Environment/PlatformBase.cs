using UnityEngine;

namespace SpringCleaning.Physics
{
    public abstract class PlatformBase : MonoBehaviour, IPhysicsMover
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

        protected float _time;

        public virtual void OnValidate()
        {
            if (_boundingEffector) _boundingEffector.isTrigger = true;
        }

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void FixedUpdate()
        {
            var newPos = Evaluate(Time.fixedDeltaTime);

            // Position
            var positionDifference = newPos - Rb.position;
            if (positionDifference.sqrMagnitude > 0)
            {
                FramePositionDelta = positionDifference;
                Rb.velocity = FramePositionDelta / Time.fixedDeltaTime;
            }
            else
            {
                FramePositionDelta = Vector3.zero;
                Rb.velocity = Vector3.zero;
            }
        }

        public void Update()
        {
            _time += Time.deltaTime;
        }

        protected abstract Vector2 Evaluate(float delta);
    }
}