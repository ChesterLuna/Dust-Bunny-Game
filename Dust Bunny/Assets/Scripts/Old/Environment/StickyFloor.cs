using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFloor : MonoBehaviour
{
    [SerializeField] float _stickyHeightDivisor = 5;
    void OnTriggerEnter2D(Collider2D collider)
    {
        OLDPlayerController player = collider.gameObject.GetComponent<OLDPlayerController>();
        if (player != null)
        {
            player.StickyHeightDivisor = player.JumpForce / _stickyHeightDivisor;
        }
    } // end OnTriggerEnter2D

    void OnTriggerExit2D(Collider2D collider)
    {
        OLDPlayerController player = collider.gameObject.GetComponent<OLDPlayerController>();
        if (player != null)
        {
            player.StickyHeightDivisor = 1;
        }
    } // end OnTriggerExit2D
} // end class StickyFloor
