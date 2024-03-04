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
    Collider2D _collider;
    [SerializeField] float offsetOnTopOfHead = 1.5f;

    [SerializeField] TextMeshPro charNameText;
    [SerializeField] TextCrawler dialogueText;
    [SerializeField] bool importantDialogue = false;
    [SerializeField] bool playOnTrigger = false;

    public Queue<Dialogue> Dialogues = new Queue<Dialogue>();

    public bool IsBubble = false;
    bool _isStartedDialogue = false;
    bool _isFinishedDialogue = false;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    } // end Awake

    private void Start()
    {
        if (AssetText == null)
        {
            Debug.LogError("No dialogue given. Try adding a .txt to the GameObject.");
            return;
        }

        Dialogues = GetComponent<TextAnalyzer>().AnalyzeText(AssetText);
    } // end Start

    public void Interact()
    {
        if (!_isStartedDialogue)
        {
            StartDialogue();
        }
        else
        {
            DisplayNextSentence();
        }
    } // end Interact

    public void StartDialogue()
    {
        _isStartedDialogue = true;
        _isFinishedDialogue = false;
        if (importantDialogue)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().PlayerState = PlayerController.PlayerStates.Dialogue;

        }
        Debug.Log("Hola");

        // If the Dialogue is supposed to be text bubble dialogue, create a text bubble and use their text boxes
        if (IsBubble)
        {
            _textBubble = Instantiate(textBubble, new Vector3(transform.position.x, transform.position.y + _collider.bounds.size.y + offsetOnTopOfHead), transform.rotation);

            charNameText = _textBubble.transform.Find("Bubble Canvas").transform.Find("Background").transform.Find("Character Name").GetComponent<TextMeshPro>();
            dialogueText = _textBubble.GetComponent<TextCrawler>().Initalize();
        }

        DisplayNextSentence();
    } // end StartDialogue


    public void DisplayNextSentence()
    {
        if (Dialogues.Count == 0 || _isFinishedDialogue)
        {
            EndDialogue();
            return;
        }

        Dialogue nextDialogue = Dialogues.Dequeue();

        charNameText.text = nextDialogue.getName();
        dialogueText.SetText(nextDialogue.getText());
        dialogueText.Advance();
    } // end DisplayNextSentence


    public void EndDialogue()
    {
        if (importantDialogue)
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().PlayerState = PlayerController.PlayerStates.Playing;
        playOnTrigger = false;
        if (IsBubble)
        {
            Destroy(_textBubble);

        }
        _isFinishedDialogue = true;
    } // end EndDialogue

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && playOnTrigger)
        {
            StartDialogue();
        }
    } // end OnTriggerEnter2D
} // end class DialogueManager
