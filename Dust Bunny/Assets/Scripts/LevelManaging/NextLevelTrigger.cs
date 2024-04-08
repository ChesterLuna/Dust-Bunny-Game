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
    [SerializeField] private bool _onTouch = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPlayerController controller) && _onTouch)
        {
            GameManager.instance.CheckpointDustLevel = controller.CurrentDust;
            ChangeScene();
        }
    } // end OnTriggerEnter2D

    public void ChangeScene()
    {
        GameManager.instance.CheckpointLocation = _nextLevelSpawnLocation;
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevel(_nextLevelName, _transition, _transitionTime);
    }
} // end class NextLevelTrigger
