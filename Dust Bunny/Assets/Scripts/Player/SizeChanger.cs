using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    [SerializeField] GameObject bunnyPlayer;
    [SerializeField] int newSize = 2;

    void Start()
    {
        bunnyPlayer = GameObject.FindWithTag("Player");
    } // end Start

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter collider");
        bunnyPlayer.GetComponent<PlayerController>().ChangeSize(newSize);

    } // end OnTriggerEnter2D

} // end class SizeChanger
