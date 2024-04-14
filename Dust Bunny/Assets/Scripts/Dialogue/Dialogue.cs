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
        string _playSound = null;
        // Unity Animations
        bool _playAnimation;
        int _animationsToPlay = 1;
        // Timeline Animations
        bool _playCinematic;

        float _timeToPlay = 0;

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
        public void setSound(string _newSound)
        {
            _playSound = _newSound;
        }
        public void setPlayAnimation(bool _newAnim)
        {
            _playAnimation = _newAnim;
        }
        public void setAnimationsToPlay(int _AnimQuantity)
        {
            _animationsToPlay = _AnimQuantity;
        }
        public void setPlayCinematic(bool _newCinematic)
        {
            _playCinematic = _newCinematic;
        }
        public void setLastDialogue(bool _newLastDialogue)
        {
            _lastDialogue = _newLastDialogue;
        }
        public void SetTimeToPlay(float _secondsToPlay)
        {
            _timeToPlay = _secondsToPlay;
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
        public string getSound()
        {
            return _playSound;
        }
        public bool isPlayAnimation()
        {
            return _playAnimation;
        }
        public bool isPlayCinematic()
        {
            return _playCinematic;
        }
        public int getAnimationsToPlay()
        {
            return _animationsToPlay;
        }

        internal bool IsLastDialogue()
        {
            return _lastDialogue;
        }
        public float GetTimeToPlay()
        {
            return _timeToPlay;
        }
    }
}
