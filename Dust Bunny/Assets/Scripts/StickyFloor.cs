using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFloor : MonoBehaviour
{
    [SerializeField] float _stickyHeightDivisor = 5;
    void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerController player = collider.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetStickyHeightDivisor(player.GetJumpForce() / _stickyHeightDivisor);
        }
    } // end OnTriggerEnter2D

    void OnTriggerExit2D(Collider2D collider)
    {
        collider.gameObject.GetComponent<PlayerController>()?.SetStickyHeightDivisor(1);
    } // end OnTriggerExit2D
} // end class StickyFloor
