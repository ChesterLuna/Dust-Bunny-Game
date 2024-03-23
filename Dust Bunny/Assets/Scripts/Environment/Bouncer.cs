using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private float _bounceForce = 60f;
    [SerializeField] private float _maxForce = 20;
    [SerializeField] private AudioClip _clip;

    [SerializeField] private bool _useSurfaceNormal;


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

        AudioSource.PlayClipAtPoint(_clip, pos, Mathf.InverseLerp(0, _maxForce, force.magnitude));
    } // end OnTriggerEnter2D
} // end class Bouncer

