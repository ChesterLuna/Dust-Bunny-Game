using System;
using System.Collections;
using System.Collections.Generic;
using Bunny.Dialogues;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
                
                // A dialogue should at least have text body. You can still have empty dialogues by only writing :
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



}
