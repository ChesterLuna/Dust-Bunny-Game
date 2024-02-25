using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RigidBodyRideable : MonoBehaviour
{
    protected List<PlayerController> _riders = new();
    protected Rigidbody2D _rb;
    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;
    }

    public void SetRider(PlayerController pc)
    {
        if (!_riders.Contains(pc)) _riders.Add(pc);
    }

    public void RemoveRider(PlayerController pc)
    {
        if (_riders.Contains(pc))
        {
            _riders.Remove(pc);
            pc.ResetParent();

        }
    }

    protected void move_with_riders()
    {

        foreach (PlayerController rider in _riders)
        {
            Debug.Log(this.name);
            rider.SetParent(transform);
        }
    }
}
