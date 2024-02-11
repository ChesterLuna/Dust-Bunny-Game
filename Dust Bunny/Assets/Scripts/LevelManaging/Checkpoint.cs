using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] private bool _useCustomLocation;
    [SerializeField] private Vector3 _spawnLocation;
    private GameManager _gameManager;

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    } // end Awake

    void Start()
    {
        if (!_useCustomLocation)
        {
            _spawnLocation = transform.position;
        }
    } // end Start(

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _gameManager.CheckpointLocation = _spawnLocation;
        }
    } // end OnTriggerEnter2D
} // end class Checkpoint
