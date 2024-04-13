using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DoIfDialogueFinished : MonoBehaviour
{
    public DialogueManager target;
    public UnityEvent onIsFinished;

    // Update is called once per frame
    void Update()
    {
        if (target != null && target.IsFinishedDialogue()){
            onIsFinished.Invoke();
        }
    }

    public void Destroy(){
        Destroy(gameObject);
    }
}
