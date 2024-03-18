using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootCollision : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            Enemy enemy = transform.parent.GetComponent<Enemy>();
            // if (enemy != null) enemy.TurnQueued = true;
        }
    } // end OnTriggerEnter2D
} // end class EnemyFootCollision 