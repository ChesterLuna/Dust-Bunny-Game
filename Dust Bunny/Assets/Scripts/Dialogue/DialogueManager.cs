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

    [SerializeField] TextMeshPro charNameText;
    [SerializeField] TextCrawler dialogueText;
    [SerializeField] bool importantDialogue = false;
    [SerializeField] bool playOnTouch = false;
    [SerializeField] bool interactable = true;
    public bool ShowIndicator { get; private set; } = false;

    public Queue<Dialogue> Dialogues = new Queue<Dialogue>();

    public bool IsBubble = false;
    public bool IsStartedDialogue = false;
    bool _isFinishedDialogue = false;

    PlayerSFXController _sfxControlller;
    [SerializeField] Animator[] _actors;
    int _iAnim = 0;

    private float _timeSinceDialogueStarted = 0.0f;

    PlayerController _player;


    private void Awake()
    {
        textBubble = textBubble != null ? textBubble : transform.Find("Text Bubble").gameObject;
        textBubble.SetActive(false);
        GameObject _playerObj = GameObject.FindWithTag("Player");
        if (_playerObj != null) _player = _playerObj.GetComponent<PlayerController>();

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
        if (!interactable) return;
        InteractDialogue();
    } // end Interact

    public void FixedUpdate()
    {
        if (UserInput.instance.Gather(PlayerStates.Dialogue).AnyKey && IsStartedDialogue && _timeSinceDialogueStarted > 0.5f)
        {
            InteractDialogue();
        }

        if (IsStartedDialogue)
        {
            _timeSinceDialogueStarted += Time.deltaTime;
        }
    }

    public void InteractDialogue()
    {
        if (!IsStartedDialogue)
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
    }

    public void StartDialogue()
    {
        IsStartedDialogue = true;
        _isFinishedDialogue = false;
        if (importantDialogue)
        {
            _player.EnableDialogue();
            StartCoroutine("WaitUntilGroundedForDialogue");
        }
        else
        {
            // If the Dialogue is supposed to be text bubble dialogue, create a text bubble and use their text boxes
            if (IsBubble)
            {
                EnableTextBubble();
            }
            EnableAnimators();

            DisplayNextSentence();
        }

    } // end StartDialogue

    IEnumerator WaitUntilGroundedForDialogue()
    {
        while (!_player.Grounded)
        {
            yield return null;
        }
        if (IsBubble)
        {
            EnableTextBubble();
        }
        EnableAnimators();
        DisplayNextSentence();
        yield break;
    }

    private void EnableTextBubble()
    {
        // _textBubble = Instantiate(textBubble, new Vector3(transform.position.x, transform.position.y + _collider.bounds.size.y + offsetOnTopOfHead), transform.rotation);
        textBubble.SetActive(true);
        charNameText = textBubble.transform.Find("Bubble Canvas").transform.Find("Background").transform.Find("Character Name").GetComponent<TextMeshPro>();
        dialogueText = textBubble.GetComponent<TextCrawler>().Initalize();
        Debug.Log(dialogueText);
    }

    private void EnableAnimators()
    {
        foreach (Animator _animator in _actors)
        {
            _animator.enabled = true;
        }
    }

    private void DisableAnimators()
    {
        foreach (Animator _animator in _actors)
        {
            _animator.enabled = false;
        }
    }

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
        {
            for (int i = 0; i < nextDialogue.getAnimationsToPlay(); i++)
            {
                PlayNextAnimation();
            }
        }
        Debug.Log(dialogueText);

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
            Debug.LogWarning("There arent enough actors (Animators) set up. Please add some into the dialogue manager or check how many times they are called in the dialogue");
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
        {
            _player.DisableDialogue();
        }
        playOnTouch = false;
        if (IsBubble)
        {
            textBubble.SetActive(false);
        }
        _isFinishedDialogue = true;
        IsStartedDialogue = false;
        ShowIndicator = false;
        DisableAnimators();
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
