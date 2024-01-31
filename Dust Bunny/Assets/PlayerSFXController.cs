using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    public enum SFX{ Jump };

    public void PlaySFX(SFX soundEffect){
        GameObject soundObject = null;
        // Junction for determining which game object this sfx represents
        switch(soundEffect){
            case SFX.Jump:
                soundObject = getChildSoundEffect("Jump");
                break;
        }

        //Check if we were able to find the sound effect
        if(soundObject == null){
            Debug.Log("Could not match a sound effect to it's child!");
            return;
        }

        //Check if the found game object has an audio source (this should always pass if the project is set up correctly)
        AudioSource audio = soundObject.GetComponent<AudioSource>();
        if(audio == null){
            Debug.Log("Matched audio source has no audio source! Found: " + soundObject.name);
        }

        //Play the requested sound effect
        float randomPitch = Random.Range(0.9f, 1.1f);
        audio.pitch = randomPitch;
        audio.Play();
    }

    //Helper to find a child sound effect based on some key; useful if we want to refactor out of using find all the time
    private GameObject getChildSoundEffect(string name){
        return GameObject.Find(name);
    }
}
