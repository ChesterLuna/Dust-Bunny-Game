using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] DialogueManager _dialoguePlayer = null;
    [SerializeField] bool playOnTouch = true;
    public bool ShowIndicator { get; private set; } = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_dialoguePlayer == null)
        {
            Debug.LogError("You didnt add a dialogue npc to the dialogue trigger");
            return;
        }
        if (other.tag == "Player" && playOnTouch)
        {
            _dialoguePlayer.StartDialogue();
            playOnTouch = false;
        }
    } // end OnTriggerEnter2D


    public void Interact()
    {
        if(_dialoguePlayer.IsStartedDialogue)
        {
            Debug.Log("Talk");
            _dialoguePlayer.InteractDialogue();
        }
    } // end Interact

}
