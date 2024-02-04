using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bunny.Dialogues;

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour, IInteractable
{
/*
    How to use:
    Make a .txt file and fill it up in this format
    #<This is the name of the character>
    :<This is the dialogue you want on the character>

    If you dont write # or : the text wont be recognized, 
    so try to always write them even if you dont want the
    character to have name or text for that dialogue.

*/
    [SerializeField] TextAsset AssetText;
    [SerializeField] GameObject textBubble;
    GameObject _textBubble;

    [SerializeField] TextMeshPro charNameText;
    [SerializeField] TextMeshPro dialogueText;

    public Queue<Dialogue> Dialogues = new Queue<Dialogue>();

    public bool IsBubble = false;
    bool _isStartedDialogue = false;
    bool _isFinishedDialogue = false;


    private void Start()
    {
        if (AssetText == null)
        {
            Debug.LogError("No dialogue given. Try adding a .txt to the GameObject.");
            return;
        }
        Dialogues = GetComponent<TextAnalyzer>().AnalyzeText(AssetText);
    }

    public void Interact()
    {
        if(!_isStartedDialogue)
        {
            StartDialogue();
        }
        else
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue()
    {
        _isStartedDialogue = true;
        _isFinishedDialogue = false;

        // If the Dialogue is supposed to be text bubble dialogue, create a text bubble and use their text boxes
        if(IsBubble)
        {
            _textBubble = Instantiate(textBubble, this.transform);

            charNameText = _textBubble.transform.Find("Character Name").GetComponent<TextMeshPro>();
            dialogueText = _textBubble.transform.Find("Bubble Text").GetComponent<TextMeshPro>();
        }

        DisplayNextSentence();
    }


    public void DisplayNextSentence()
    {
        if (Dialogues.Count == 0 || _isFinishedDialogue)
        {
            EndDialogue();
            return;
        }

        Dialogue nextDialogue = Dialogues.Dequeue();

        charNameText.text = nextDialogue.getName();
        dialogueText.text = nextDialogue.getText();
    }


    public void EndDialogue()
    {

        if(IsBubble)
        {
            Destroy(_textBubble);

        }
        _isFinishedDialogue = true;

    }


}
