using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bunny.Dialogues;

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
    [SerializeField] TextMeshPro dialogueText;
    [SerializeField] bool importantDialogue = false;
    [SerializeField] bool playOnTouch = false;
    [SerializeField] bool interactable = true;
    [SerializeField] float _timeToPlay = 0;

    [SerializeField] float minDust = -1;
    [SerializeField] float maxDust = 1000;

    public bool ShowIndicator { get; private set; } = false;

    public Queue<Dialogue> Dialogues = new Queue<Dialogue>();

    public bool IsBubble = false;
    public bool IsStartedDialogue = false;
    public UnityEvent onFinishedDialogue;
    bool _isFinishedDialogue;

    [Header("Animations")]
    PlayerSFXController _sfxControlller;
    [SerializeField] Animator[] _actors;
    int _iAnim = 0;
    [SerializeField] TimelineAsset[] _cinematics;
    int _iCine = 0;

    private float _timeSinceDialogueStarted = 0.0f;

    PlayerController _player;

    private void Awake()
    {
        textBubble = textBubble != null ? textBubble : transform.Find("Text Bubble").gameObject;
        textBubble.SetActive(false);
        GameObject _playerObj = GameObject.FindWithTag("Player");
        if (_playerObj != null) _player = _playerObj.GetComponent<PlayerController>();
        _isFinishedDialogue = GetComponent<PersistentGUID>().LoadBoolValue("isFinishedDialogue");
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
        // if (!interactable) return;
        // InteractDialogue();
    } // end Interact

    public bool IsFinishedDialogue()
    {
        return _isFinishedDialogue;
    }

    public void FixedUpdate()
    {
        if (UserInput.instance.Gather(PlayerStates.Dialogue).InteractDown && IsStartedDialogue && _timeSinceDialogueStarted > 0.5f && interactable)
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
        // else if (!dialogueText.IsFinishedLine())
        // {
        //     dialogueText.FinishLine();
        // }
        else
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue()
    {
        if (_isFinishedDialogue == true || !IsEnoughDust()) return;
        IsStartedDialogue = true;
        _isFinishedDialogue = false;
        if (importantDialogue)
        {
            _player.EnableDialogue();
            StartCoroutine("WaitUntilGroundedForDialogue");
        }
        else
        {
            SetUpDialogueSystem();
        }

    } // end StartDialogue

    IEnumerator WaitUntilGroundedForDialogue()
    {
        while (!_player.Grounded)
        {
            yield return null;
        }
        Invoke("SetUpDialogueSystem", _timeToPlay);
        yield break;
    }

    void SetUpDialogueSystem()
    {
        if (IsBubble)
        {
            EnableTextBubble();
        }
        EnableAnimators();
        DisplayNextSentence();
    }

    private void EnableTextBubble()
    {
        // _textBubble = Instantiate(textBubble, new Vector3(transform.position.x, transform.position.y + _collider.bounds.size.y + offsetOnTopOfHead), transform.rotation);
        textBubble.SetActive(true);
        charNameText = textBubble.transform.Find("Bubble Canvas").transform.Find("Background").transform.Find("Character Name").GetComponent<TextMeshPro>();
        dialogueText = textBubble.transform.Find("Bubble Canvas").transform.Find("Background").transform.Find("Bubble Text").GetComponent<TextMeshPro>();
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
        if (nextDialogue.isPlayCinematic())
            PlayNextCinematic();


        charNameText.text = nextDialogue.getName();
        dialogueText.text = nextDialogue.getText();
        Debug.Log(nextDialogue.getText());
        Debug.Log(dialogueText.text);
        // dialogueText.SetText(nextDialogue.getText());
        // dialogueText.Advance();
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
        if (_nextActor.gameObject == _player.gameObject)
        {
            _player._lastAnimationState++;
            _nextActor.SetInteger("PlayerAnimationTrigger", _player._lastAnimationState);
        }
        _nextActor.SetTrigger("DialogueTrigger");
        _iAnim++;
    }
    public void PlayNextCinematic()
    {
        Debug.Log("Play a cinematic");

        //Find next animator
        if (_iCine >= _cinematics.Length)
        {
            Debug.LogWarning("There arent enough timelines (Cinematics) set up. Please add some into the dialogue manager or check how many times they are called in the dialogue");
            return;
        }
        TimelineAsset _nextCine = _cinematics[_iCine];

        // Play Cinematic fx
        GetComponent<PlayableDirector>().Play(_nextCine);

        _iCine++;
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
        GetComponent<PersistentGUID>().SaveBoolValue("isFinishedDialogue", _isFinishedDialogue);
        ES3.Save("_lastAnimationState", _player._lastAnimationState); // This is saved but doesnt seem to be loaded anywhere?

        IsStartedDialogue = false;
        ShowIndicator = false;
        DisableAnimators();
        onFinishedDialogue.Invoke();
    } // end EndDialogue

    public bool IsEnoughDust()
    {
        return minDust <= _player.CurrentDust && _player.CurrentDust < maxDust;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && playOnTouch)
        {

            StartDialogue();
            playOnTouch = false;
        }
    } // end OnTriggerEnter2D
} // end class DialogueManager
