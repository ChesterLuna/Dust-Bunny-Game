using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class EnemySideCollision : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("EnemySideCollision: OnTriggerEnter2D: other.gameObject.name: " + other.gameObject.name);
            EnemyMovement enemyMovement = transform.parent.GetComponent<EnemyMovement>();
            if (enemyMovement != null) enemyMovement.TurnQueued = true;

            RidableEnemy ridableEnemy = transform.parent.GetComponent<RidableEnemy>();
            if (ridableEnemy != null) ridableEnemy.TurnQueued = true;
        }
    }
}

