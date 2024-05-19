using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

  public void StartLoadLevel(string LevelName, Animator transition, float transitionTime = 1f)
  {
    StartCoroutine(LoadLevel(LevelName, transition, transitionTime));
  } // end StartLoadLevel

  IEnumerator LoadLevel(string LevelName, Animator transition, float transitionTime)
  {
    transition.SetTrigger("Start");

    yield return new WaitForSeconds(transitionTime);
    SceneManager.LoadScene(LevelName);
  } // end IEnumerator LoadLevel

} // end class LevelLoader
