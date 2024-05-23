using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpringCleaning.Player;
public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string _nextLevelName;
    [SerializeField] private Vector3 _nextLevelSpawnLocation;
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 1f;
    [SerializeField] private bool _onTouch = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPlayerController controller))
        {
            GameManager.Instance.CheckpointDustLevel = controller.CurrentDust;
            if (_onTouch) ChangeScene();
        }
    } // end OnTriggerEnter2D

    public void ChangeScene()
    {
        GameManager.Instance.CheckpointLocation = _nextLevelSpawnLocation;
        ES3AutoSaveMgr.Current.Save();
        GameObject.FindWithTag("Player").GetComponent<PlayerController>()._lastAnimationState = 0;
        if (_nextLevelName == "Good Ending")
        {
            GameManager.Instance.StopGameTime();
            GameManager.Instance.CheckpointDustLevel = -1;
        }

        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevel(_nextLevelName, _transition, _transitionTime);
    }
    public void ChangeSceneString(string nextLevelString)
    {
        GameManager.Instance.CheckpointLocation = _nextLevelSpawnLocation;
        ES3AutoSaveMgr.Current.Save();
        GameObject.FindWithTag("Player").GetComponent<PlayerController>()._lastAnimationState = 0;
        if (nextLevelString == "Good Ending")
        {
            GameManager.Instance.StopGameTime();
            GameManager.Instance.CheckpointDustLevel = -1;
        }

        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevel(nextLevelString, _transition, _transitionTime);
    }

} // end class NextLevelTrigger
