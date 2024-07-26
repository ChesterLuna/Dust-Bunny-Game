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

  public void StartLoadLevelByString(string LevelName, string transition, float transitionTime = 1f)
  {
    StartCoroutine(LoadLevelByString(LevelName, transition, transitionTime));
  } // end StartLoadLevel

  IEnumerator LoadLevel(string LevelName, Animator transition, float transitionTime)
  {
    //Wait for a bit to make sure inputs get cleared
    yield return new WaitForSeconds(0.2f);
    if(transition != null){
      transition.SetTrigger("Start");
    }

    // dumb fix to make sure controllers dont keep rumbling after a scene switch
    if (Gamepad.all.Count > 0){
      Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }

    yield return new WaitForSeconds(transitionTime);
    PhysicsSimulator.Instance.ClearPhysicsObjects();
    SceneManager.LoadScene(LevelName);
  } // end IEnumerator LoadLevel

  IEnumerator LoadLevelByString(string LevelName, string transition, float transitionTime){
    Animator anim = null;
    Transform transitionAnimationsContainer = transform.Find("TransitionAnimations");
    Transform transitionObj = transitionAnimationsContainer.transform.Find(transition);
    if(transitionObj == null){
      Debug.Log("Transition not found: " + transition);
    } else {
      anim = transitionObj.gameObject.GetComponent<Animator>();
    }

    yield return LoadLevel(LevelName, anim, transitionTime);
  }

} // end class LevelLoader
