using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Switchable : MonoBehaviour
{
    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    public virtual void Enable()
    {
        gameObject.SetActive(true);
    }
}
