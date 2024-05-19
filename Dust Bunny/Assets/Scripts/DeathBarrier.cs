using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpringCleaning.Player;

namespace SpringCleaning.Environment
{
    public class DeathBarrier : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out IPlayerController controller)) return;

            controller.Die();

        } // end OnTriggerEnter2D
    } // end DeathBarrier
}
