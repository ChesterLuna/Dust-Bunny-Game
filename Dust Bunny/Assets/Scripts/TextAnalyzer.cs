using System.Collections;
using System.Collections.Generic;
using Bunny.Dialogues;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextAnalyzer : MonoBehaviour
{

    //DialogueManager manager;
    // Start is called before the first frame update
    private void Awake()
    {
        //manager = FindObjectOfType<DialogueManager>();
        //txt = AssetText.text;
        //lines = txt.Split(System.Environment.NewLine.ToCharArray());


    }
    void Start()
    {
        // Debug.Log("Splitted text");

    }

    public Queue<Dialogue> AnalyzeText(TextAsset AssetText)
    {
        int i = 0;


        string txt = AssetText.text;
        string[] lines = txt.Split(System.Environment.NewLine.ToCharArray());
        foreach( string line in lines)
        {
            print(line);
        }
        Queue<Dialogue> Dialogues = new Queue<Dialogue>();
        string _Name = null;
        string _Text = null;

        while (i < lines.Length)
        {
            if (!string.IsNullOrEmpty(lines[i]))
            {

                if (lines[i][0] == '#')
                {
                    _Name = lines[i].Remove(0, 1);

                }
                if (lines[i][0] == ':')
                {
                    _Text = lines[i].Remove(0, 1);
                }
                //print(lines[i]);
                
                if(_Text != null)
                {
                    Dialogues.Enqueue(new Dialogue(_Name, _Text));
                    _Name = null;
                    _Text = null;

                }
            }
            i++;
        }
        return Dialogues;
    }


    /*
    //If we had choices we could use this code
    public void AnalyzeChoice(char choice)
    {

        while (i < lines.Length)
        {
            if (!string.IsNullOrEmpty(lines[i]))
            {
                if (lines[i][0] == ']')
                {
                    // Debug.Log("Reconoce la ]");
                    i++;
                    AnalyzeText();
                    manager.DisplayNextSentence();
                    break;
                }
                if(lines[i][0] == choice)
                {
                    if (lines[i][1] == '@')
                    {
                        manager.names.Enqueue(lines[i].Remove(0, 1));
                    }
                    if (lines[i][1] == '#')
                    {
                        if (lines[i].Remove(0, 2) == "")
                            manager.names.Enqueue(" ");
                        else
                            manager.names.Enqueue(lines[i].Remove(0, 2));

                        //manager.names.Enqueue(lines[i].Remove(0, 2));
                    }
                    if (lines[i][1] == ':')
                    {
                        manager.dialogues.Enqueue(lines[i].Remove(0, 2));
                    }
                    // if (lines[i][1] == '$')
                    // { ANIMATION
                    //     manager.dialogues.Enqueue(lines[i].Remove(0, 2));
                    // }

                }
            }
            i++;
        }

        //for(ind=1; ind < nOfChoices; nOfChoices;)

    }
    */

    // public void StopChoice()
    // {
    // }







}
