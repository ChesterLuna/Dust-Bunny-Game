using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bunny.Dialogues
{
    public class Dialogue
    {
        string _name;
        string _text;
        bool _bubble;
        bool _lastDialogue;

        public Dialogue(string theName, string theText, bool isBubble = false, bool isLastDialogue = false)
        {
            // Leave it empty if there is no given name
            if (theName != "")
            {
                _name = theName;
            }
            else
            {
                _name = " ";
            }
            _text = theText;
            _bubble = isBubble;
            _lastDialogue = isLastDialogue;
        }
        public string getName()
        {
            return _name;
        }
        public string getText()
        {
            return _text;

        }
        public bool isBubble()
        {
            return _bubble;
        }

        internal bool IsLastDialogue()
        {
            return _lastDialogue;
        }
    }
}