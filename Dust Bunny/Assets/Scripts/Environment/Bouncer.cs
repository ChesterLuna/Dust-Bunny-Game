using UnityEngine;
using SpringCleaning.Player;

namespace SpringCleaning.Physics
{
    public class Bouncer : MonoBehaviour
    {
        [Tooltip("The force applied to the player when they bounce on this object"), SerializeField]
        private float _bounceForce = 60f;

        [Tooltip("The magnitude limit of the force applied to the player"), SerializeField]
        private float _maxForce = 20;

        [Tooltip("Optional clip to play when a bounce occurs")]
        private AudioClip _clip;

        [Tooltip("If true, the bounce force will be perpendicular to the surface normal. If false, it will be perpendicular to the incoming velocity."), SerializeField]
        private bool _useSurfaceNormal;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out IPlayerController controller)) return;

            var pos = transform.position;
            Vector2 force;

            if (_useSurfaceNormal)
            {
                var collisionPoint = collision.ClosestPoint(pos);
                var collisionNormal = pos - (Vector3)collisionPoint;
                force = -collisionNormal;
            }
            else
            {
                var incomingSpeedNormal = Vector3.Project(controller.Velocity, transform.up);
                force = -incomingSpeedNormal;
            }

            force = Vector2.ClampMagnitude(force * _bounceForce, _maxForce);

            controller.AddFrameForce(force, true);

            if (_clip != null) AudioSource.PlayClipAtPoint(_clip, pos, Mathf.InverseLerp(0, _maxForce, force.magnitude));
        } // end OnTriggerEnter2D
    } // end class bouncer
}