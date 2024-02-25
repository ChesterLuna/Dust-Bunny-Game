using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechVoice : MonoBehaviour{
    public List<AudioClip> letterClips;

    private AudioSource _source;

    void Awake(){
        _source = GetComponent<AudioSource>();
    }

    public AudioClip GetLetterClip(char letter){
        int index = char.ToUpper(letter) - 65; // convert this char into an index in the letters array based on it's position in ASCII

        if(index < 0 || index > 26) return null; // out of bounds check

        //Return the correct audio clip for this letter
        return letterClips[index];
    }
}