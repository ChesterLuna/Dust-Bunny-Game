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
        bool _playSound;
        bool _playAnimation;

        public Dialogue(string _theName, string _theText, bool _isBubble = false, bool _isLastDialogue = false)
        {
            // Leave it empty if there is no given name
            if (_theName != "")
            {
                _name = _theName;
            }
            else
            {
                _name = " ";
            }
            _text = _theText;
            _bubble = _isBubble;
            _lastDialogue = _isLastDialogue;
        }

        public void setName(string _newName)
        {
            _name = _newName;
        }
        public void setText(string _newText)
        {
            _text = _newText;
        }
        public void setBubble(bool _newBubble)
        {
            _bubble = _newBubble;
        }
        public void setPlaySound(bool _newSound)
        {
            _playSound = _newSound;
        }
        public void setPlayAnimation(bool _newAnim)
        {
            _playAnimation = _newAnim;
        }
        public void setLastDialogue(bool _newLastDialogue)
        {
            _lastDialogue = _newLastDialogue;
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
        public bool isPlaySound()
        {
            return _playSound;
        }
        public bool isPlayAnimation()
        {
            return _playAnimation;
        }
        internal bool IsLastDialogue()
        {
            return _lastDialogue;
        }
    }
}
