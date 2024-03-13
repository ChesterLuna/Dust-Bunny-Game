using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string _nextLevelName;
    [SerializeField] private Vector3 _nextLevelSpawnLocation;
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.CheckpointLocation = _nextLevelSpawnLocation;

            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            levelLoader.StartLoadLevel(_nextLevelName, _transition, _transitionTime);
        }
    } // end OnTriggerEnter2D
} // end class NextLevelTrigger
