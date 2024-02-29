using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemySideCollision : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            Debug.Log("EnemySideCollision: OnTriggerEnter2D: other.gameObject.name: " + other.gameObject.name);
            transform.parent.GetComponent<EnemyMovement>()?.Turn();
            transform.parent.GetComponent<RidableEnemy>()?.Turn();
        }
    }
}

