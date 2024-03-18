using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationChildHelper : MonoBehaviour
{
    private PlayerAnimator _animator;

    private void Awake()
    {
        _animator = GetComponentInParent<PlayerAnimator>();
    }

    public void TellParentPlayFootstep()
    {
        _animator.OnPlayerFootstep();
    }
}
