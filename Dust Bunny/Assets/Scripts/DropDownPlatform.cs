using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DropDownPlatform : MonoBehaviour
{
    [SerializeField] private Collider2D hitbox;


    public void DropDown(Collider2D other)
    {
        Physics2D.IgnoreCollision(hitbox, other, true);
    }// end DropDown

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(hitbox, other, false);
        }
    } // end OnTriggerExit2D
}