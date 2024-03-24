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

    public void DropDown(Collider2D collision)
    {
        // Physics2D.IgnoreCollision(_hitbox, collision, true);
        _hitbox.enabled = false;
    }// end DropDown

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;
        // Physics2D.IgnoreCollision(_hitbox, collision, false);
        _hitbox.enabled = true;


    } // end OnTriggerExit2D
}