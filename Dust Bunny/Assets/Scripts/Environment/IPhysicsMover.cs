using System.Collections.Generic;
using UnityEngine;

namespace SpringCleaning.Physics
{
    /// <summary>
    /// Interface for physics movers.
    /// Something that the player can stand on and move with.
    /// </summary>
    public interface IPhysicsMover
    {
        public bool UsesBounding { get; }

        [Tooltip("The player requires grounding on the platform at least once before the grounding effector takes effect")]
        public bool RequireGrounding { get; }

        public Vector2 FramePositionDelta { get; }
        public Vector2 FramePosition { get; }
        public Vector2 Velocity { get; }
        public Vector2 TakeOffVelocity { get; }
    }
}