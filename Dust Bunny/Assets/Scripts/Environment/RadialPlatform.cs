using UnityEngine;

namespace SpringCleaning.Physics
{
    public class RadialPlatform : PlatformBase
    {
        [SerializeField] private float _radius = 5;
        [SerializeField] private float _speed = 5;

        private Vector2 _startPosition;

        protected override void Awake()
        {
            base.Awake();
            _startPosition = Rb.position;
        }

        protected override Vector2 Evaluate(float delta)
        {
            return _startPosition + new Vector2(Mathf.Cos(_time * _speed), Mathf.Sin(_time * _speed)) * _radius;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var center = Application.isPlaying ? _startPosition : (Vector2)transform.position;
            UnityEditor.Handles.DrawWireDisc(center, Vector3.back, _radius);
        }
#endif
    }
}