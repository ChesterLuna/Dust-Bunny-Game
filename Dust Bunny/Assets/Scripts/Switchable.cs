using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Switchable : MonoBehaviour
{
    public virtual void disable()
    {
        gameObject.SetActive(false);
    }

    public virtual void enable()
    {
        gameObject.SetActive(true);
    }

}
