using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyFootCollision : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("EnemyFootCollision: OnTriggerExit2D: other.gameObject.name: " + other.gameObject.name);
            transform.parent.GetComponent<EnemyMovement>()?.Turn();
            transform.parent.GetComponent<RidableEnemy>()?.Turn();
        }
    }
}
