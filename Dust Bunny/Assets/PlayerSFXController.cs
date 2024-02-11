using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    public enum SFX { Jump, Dust_Collect_Start, Dust_Collect_Stop };

    public Vector2 randomPitchVariationRange;

    public void PlaySFX(SFX soundEffect)
    {
        GameObject soundObject = null;
        float length = 0.0f;
        float playbackPosition = 0.0f;
        // Junction for determining which game object this sfx represents
        switch (soundEffect)
        {
            case SFX.Jump:
                soundObject = getChildSoundEffect("Jump");
                PlaySoundObject(soundObject);
                break;

            case SFX.Dust_Collect_Start:
                //Play intro
                soundObject = getChildSoundEffect("Dust Collect Intro");
                PlaySoundObject(soundObject);
                length = GetSourceFromObject(soundObject).clip.length;

                //Queue Loop
                soundObject = getChildSoundEffect("Dust Collect Loop");
                GetSourceFromObject(soundObject).loop = true;
                QueueSoundObject(soundObject, length);
                break;

            case SFX.Dust_Collect_Stop:
                //Stop Loop
                soundObject = getChildSoundEffect("Dust Collect Loop");
                AudioSource source = GetSourceFromObject(soundObject);
                source.loop = false;
                length = source.clip.length;
                playbackPosition = source.time;

                //Queue the stop sfx
                soundObject = getChildSoundEffect("Dust Collect End");
                QueueSoundObject(soundObject, length - playbackPosition);
                break;

            default:
                Debug.Log("Requested to play an unimplemented sound effect...");
                break;
        }
    }

    private void PlaySoundObject(GameObject soundObject){
        AudioSource audio = GetSourceFromObject(soundObject);

        //Play the requested sound effect
        float randomPitch = Random.Range(randomPitchVariationRange.x, randomPitchVariationRange.y);
        audio.pitch = randomPitch;
        audio.Play();
    }

    private void QueueSoundObject(GameObject soundObject, float delay){
        AudioSource audio = GetSourceFromObject(soundObject);

        //Queue the requested sound effect
        float randomPitch = Random.Range(randomPitchVariationRange.x, randomPitchVariationRange.y);
        audio.pitch = randomPitch;
        audio.PlayDelayed(delay - 0.1f);
    }

    //Helper to get an audio source from a game object, with safety checks
    private AudioSource GetSourceFromObject(GameObject soundObject){
        //Check if we were able to find the sound effect
        if (soundObject == null)
        {
            Debug.Log("Could not match a sound effect to it's child!");
            return null;
        }

        //Check if the found game object has an audio source (this should always pass if the project is set up correctly)
        AudioSource audio = soundObject.GetComponent<AudioSource>();
        if (audio == null)
        {
            Debug.Log("Matched audio source has no audio source! Found: " + soundObject.name);
        }

        return audio;
    }

    //Helper to find a child sound effect based on some key; useful if we want to refactor out of using find all the time
    private GameObject getChildSoundEffect(string name)
    {
        return GameObject.Find(name);
    }
}
