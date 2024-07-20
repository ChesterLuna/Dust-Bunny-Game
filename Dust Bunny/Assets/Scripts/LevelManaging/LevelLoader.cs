using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelLoader : MonoBehaviour
{

  public void StartLoadLevel(string LevelName, Animator transition, float transitionTime = 1f)
  {
    StartCoroutine(LoadLevel(LevelName, transition, transitionTime));
  } // end StartLoadLevel

  IEnumerator LoadLevel(string LevelName, Animator transition, float transitionTime)
  {
    transition.SetTrigger("Start");

    // dumb fix to make sure controllers dont keep rumbling after a scene switch
    if (Gamepad.all.Count > 0){
      Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }

    yield return new WaitForSeconds(transitionTime);
    PhysicsSimulator.Instance.ClearPhysicsObjects();
    SceneManager.LoadScene(LevelName);
  } // end IEnumerator LoadLevel

} // end class LevelLoader
