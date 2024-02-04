using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;



public class DropDownPlatform : MonoBehaviour
{
    private Collider2D _hitbox;

    void Awake()
    {
        _hitbox = transform.parent.GetComponentInParent<Collider2D>();
        if (_hitbox == null) _hitbox = GetComponent<Collider2D>();
    } // end Awake
    public void DropDown(Collider2D other)
    {
        Physics2D.IgnoreCollision(_hitbox, other, true);
    }// end DropDown

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(_hitbox, other, false);
        }
    } // end OnTriggerExit2D
}