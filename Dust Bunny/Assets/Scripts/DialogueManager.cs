using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bunny.Dialogues;


//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//using static TextAnalyzer;

public class DialogueManager : MonoBehaviour, IInteractable
{
    [SerializeField] TextAsset AssetText;
    [SerializeField] GameObject textBubble;
    GameObject _textBubble;

    [SerializeField] TextMeshPro charNameText;
    [SerializeField] TextMeshPro dialogueText;
    //[SerializeField] Canvas choiceCanvas;
    //int _choicesDone = 0;
    //int _animationsDone = 0;
    //[SerializeField] GameObject fader;

    public Queue<Dialogue> Dialogues = new Queue<Dialogue>();

    public bool IsBubble = false;
    public bool IsStartedDialogue = false;
    public bool IsFinishedDialogue = false;

    // Start is called before the first frame update
    void Awake()
    {
    }
    

    private void Start()
    {

        // if (choiceCanvas == null)
        // {
        //     if(GameObject.Find("Choice Canvas") != null)
        //     {
        //         choiceCanvas = GameObject.Find("Choice Canvas").GetComponent<Canvas>();
        //     }
        // }

        Dialogues = FindObjectOfType<TextAnalyzer>().AnalyzeText(AssetText);
        //StartDialogue();
    }

    public void Interact()
    {
        Debug.Log("HAS INTERACTED");
        if(!IsStartedDialogue)
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
        IsStartedDialogue = true;

        if (IsFinishedDialogue == false)
        {
            Debug.Log("Start Dialogue");


            if(IsBubble)
            {
                _textBubble = Instantiate(textBubble);

                charNameText = _textBubble.transform.Find("Character Name").GetComponent<TextMeshPro>();
                dialogueText = _textBubble.transform.Find("Bubble Text").GetComponent<TextMeshPro>();
            }

            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if(IsFinishedDialogue == true)
        {
            return;
        }
        if (Dialogues.Count == 0 )
        {
            EndDialogue();
            return;
        }

        Dialogue nextDialogue = Dialogues.Dequeue();


        // This is code for choices, adding portraits
        /*
        while(true)
        {
            if (nextName[0] == '@')
            {
                FindObjectOfType<PictureHandler>().UpdatePortrait((int)Char.GetNumericValue(nextName[1]));
                nextName = names.Dequeue();
                continue;
            }
            if (nextName == "Animation")
            {
                DisplayNextAnimation();
                nextName = names.Dequeue();

                continue;
            }
            break;
        }
        if (nextName == "Choice")
        {
            DisplayNextChoice();
            return;
        }

        nextName = ChangeName(nextName);
        nextSentence = ChangeName(nextSentence);

        nextName = nextName.Replace("Y/N", GameSession.Instance.NameOfProtagonist);
        nextSentence = nextSentence.Replace("Y/N", GameSession.Instance.NameOfProtagonist);
        */

        charNameText.text = nextDialogue.getName();
        dialogueText.text = nextDialogue.getText();
    }



    /*
    //If we had choices, we could use part of this code.
    public void DisplayNextChoice()
    {
        choicesDone++;
        string theChoice = "Decision " + choicesDone.ToString();

        choiceCanvas.transform.Find("Background Choice").GameObject().SetActive(true);
        choiceCanvas.transform.Find(theChoice).GameObject().SetActive(true);

    }*/

    /*
    //If we had animations, we could use part of this code.

    public void DisplayNextAnimation()
    {
        GameObject[] objectsWithAnimation = GameObject.FindGameObjectsWithTag("Animation");

        foreach (GameObject theObject in objectsWithAnimation)
        {
            List<string> states = new List<string>();
            foreach (AnimationState state in theObject.GetComponent<Animation>())
            {
                states.Add(state.name);
            }

            theObject.GetComponent<Animation>().Play(states[0]);
        }
        animationsDone++;
    }
    */

    public void EndDialogue()
    {
        if(IsBubble)
        {
            Destroy(_textBubble);

        }
        IsFinishedDialogue = true;

        // Maybe add fading music


    }


}
