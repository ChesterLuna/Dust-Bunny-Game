using UnityEngine;
using SpringCleaning.Player;

namespace SpringCleaning.Physics
{
    public class MovementEffector : MonoBehaviour, ISpeedModifier
    {
        [field: SerializeField] public bool InAir { get; private set; }
        [field: SerializeField] public bool OnGround { get; private set; }
        [field: SerializeField] public Vector2 Modifier { get; private set; } = new(-0.5f, 0);
    } // end class MovementEffector
}