using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    public enum SFX { Jump, Dust_Collect_Start, Dust_Collect_Stop };

    public Vector2 randomPitchVariationRange;

    public void PlaySFX(SFX soundEffect)
    {
        GameObject _soundObject = null;
        float _length = 0.0f;
        float _playbackPosition = 0.0f;
        // Junction for determining which game object this sfx represents
        switch (soundEffect)
        {
            case SFX.Jump:
                _soundObject = GetChildSoundEffect("Jump");
                PlaySoundObject(_soundObject);
                break;

            case SFX.Dust_Collect_Start:
                //Play intro
                _soundObject = GetChildSoundEffect("Dust Collect Intro");
                PlaySoundObject(_soundObject);
                _length = GetSourceFromObject(_soundObject).clip.length;

                //Queue Loop
                _soundObject = GetChildSoundEffect("Dust Collect Loop");
                GetSourceFromObject(_soundObject).loop = true;
                QueueSoundObject(_soundObject, _length);
                break;

            case SFX.Dust_Collect_Stop:
                //Stop Loop
                _soundObject = GetChildSoundEffect("Dust Collect Loop");
                AudioSource source = GetSourceFromObject(_soundObject);
                source.loop = false;
                _length = source.clip.length;
                _playbackPosition = source.time;

                //Queue the stop sfx
                _soundObject = GetChildSoundEffect("Dust Collect End");
                QueueSoundObject(_soundObject, _length - _playbackPosition);
                break;

            default:
                Debug.Log("Requested to play an unimplemented sound effect...");
                break;
        }
    }

    private void PlaySoundObject(GameObject _soundObject){
        AudioSource _audio = GetSourceFromObject(_soundObject);

        //Play the requested sound effect
        float _randomPitch = Random.Range(randomPitchVariationRange.x, randomPitchVariationRange.y);
        _audio.pitch = _randomPitch;
        _audio.Play();
    }

    private void QueueSoundObject(GameObject _soundObject, float _delay){
        AudioSource _audio = GetSourceFromObject(_soundObject);

        //Queue the requested sound effect
        float _randomPitch = Random.Range(randomPitchVariationRange.x, randomPitchVariationRange.y);
        _audio.pitch = _randomPitch;
        _audio.PlayDelayed(_delay - 0.1f);
    }

    //Helper to get an audio source from a game object, with safety checks
    private AudioSource GetSourceFromObject(GameObject _soundObject){
        //Check if we were able to find the sound effect
        if (_soundObject == null)
        {
            Debug.Log("Could not match a sound effect to it's child!");
            return null;
        }

        //Check if the found game object has an audio source (this should always pass if the project is set up correctly)
        AudioSource _audio = _soundObject.GetComponent<AudioSource>();
        if (_audio == null)
        {
            Debug.Log("Matched audio source has no audio source! Found: " + _soundObject.name);
        }

        return _audio;
    }

    //Helper to find a child sound effect based on some key; useful if we want to refactor out of using find all the time
    private GameObject GetChildSoundEffect(string _name)
    {
        return GameObject.Find(_name);
    }
}
