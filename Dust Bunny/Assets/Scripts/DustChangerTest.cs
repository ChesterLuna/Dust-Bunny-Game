using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustChangerTest : MonoBehaviour
{
    [SerializeField] private float dustAmount = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;
        controller.ChangeDust(dustAmount);
    }
}

