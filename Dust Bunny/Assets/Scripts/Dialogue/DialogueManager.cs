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
        $<Add this key to play a sound, please describe it in the dialogue because it has to be implemented by a programmer>
        !<Add this key to play an animation, please describe it in the dialogue because it has to be implemented by a programmer>
        #<This is the name of the character>
        :<This is the dialogue you want on the character>

        If you dont write # or : the text wont be recognized, 
        so try to always write them even if you dont want the
        character to have name or text for that dialogue.
        The sound and dialogue must always appear before  : .

    */
    [SerializeField] TextAsset AssetText;
    [SerializeField] GameObject textBubble;
    GameObject _textBubble;
    Collider2D _collider;
    [SerializeField] float offsetOnTopOfHead = 1.5f;

    [SerializeField] TextMeshPro charNameText;
    [SerializeField] TextCrawler dialogueText;
    [SerializeField] bool importantDialogue = false;
    [SerializeField] bool playOnTouch = false;
    public bool ShowIndicator { get; private set; } = false;

    public Queue<Dialogue> Dialogues = new Queue<Dialogue>();

    public bool IsBubble = false;
    bool _isStartedDialogue = false;
    bool _isFinishedDialogue = false;

    PlayerSFXController _sfxControlller;
    [SerializeField] Animator[] _actors;
    int _iAnim = 0;


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

        _sfxControlller = gameObject.GetComponentInChildren<PlayerSFXController>();
    } // end Start

    public void Interact()
    {
        if (!_isStartedDialogue)
        {
            StartDialogue();
        }
        else if (!dialogueText.IsFinishedLine())
        {
            dialogueText.FinishLine();
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
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().PlayerState = PlayerStates.Dialogue;

        }

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

        if (nextDialogue.getSound() != null)
            PlaySound(nextDialogue.getSound());
        if (nextDialogue.isPlayAnimation())
            PlayNextAnimation();

        charNameText.text = nextDialogue.getName();
        dialogueText.SetText(nextDialogue.getText());
        dialogueText.Advance();
    } // end DisplayNextSentence


    public void PlaySound(string _soundToPlay)
    {
        Debug.Log("Play a sound");

        _sfxControlller.PlaySFXByString(_soundToPlay);

        // Play audio clip sfx
        Debug.Log("Now trying to play: " + _soundToPlay);
    }
    public void PlayNextAnimation()
    {

        Debug.Log("Play an animation");

        //Find next animator
        if (_iAnim >= _actors.Length)
        {
            Debug.LogError("There arent enough actors (Animators) set up. Please add some into the dialogue manager or check how many times they are called in the dialogue");
            return;
        }
        Animator _nextActor = _actors[_iAnim];

        // Play animation fx
        _nextActor.SetTrigger("DialogueTrigger");
        _iAnim++;
    }

    public void EndDialogue()
    {
        if (importantDialogue)
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().PlayerState = PlayerStates.Playing;
        playOnTouch = false;
        if (IsBubble)
        {
            Destroy(_textBubble);

        }
        _isFinishedDialogue = true;
        _isStartedDialogue = false;
        ShowIndicator = false;
    } // end EndDialogue

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && playOnTouch)
        {
            StartDialogue();
            playOnTouch = false;
        }
    } // end OnTriggerEnter2D
} // end class DialogueManager
