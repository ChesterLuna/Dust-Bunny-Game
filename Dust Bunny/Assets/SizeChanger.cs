using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChanger : MonoBehaviour
{

    [SerializeField] GameObject bunnyPlayer;
    [SerializeField] int newSize = 2;

    // Start is called before the first frame update
    void Start()
    {
        bunnyPlayer = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter collider");
        bunnyPlayer.GetComponent<PlayerController>().ChangeSize(newSize);

    }

}
