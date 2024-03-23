using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    public enum SFX { Jump, Dust_Collect_Start, Dust_Collect_Stop_Clean, Dust_Collect_Stop_Abrupt, Foot_Step, Land, Dash, Dead, Rumble, Rattle };
    List<GameObject> _soundEffects = new List<GameObject>();
    private List<string> _soundEffectsNames = new List<string>();
    public Vector2 randomPitchVariationRange;
    // private int _stepIndex;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            _soundEffects.Add(child.gameObject);
            _soundEffectsNames.Add(child.gameObject.name);
        }
    }

    // This funciton is called by the Walking animation
    public void PlayFootstep()
    {
        PlaySFX(SFX.Foot_Step);
    } // end PlayFootstep


    public void PlayWallClimbSound()
    {
        // private int _wallClimbAudioIndex = 0;
        // _wallClimbAudioIndex = (_wallClimbAudioIndex + 1) % _wallClimbClips.Length;
        PlaySFX(SFX.Foot_Step);
    }

    public void PlayLadderClimbSound()
    {
        // private int _ladderClimbAudioIndex;
        // _ladderClimbAudioIndex = (_ladderClimbAudioIndex + 1) % _ladderClimbClips.Length;
        PlaySFX(SFX.Foot_Step);
    }

    public void PlaySFX(SFX soundEffect)
    {
        //Local variables
        GameObject _soundObject = null;
        AudioSource _source = null;
        float _length = 0.0f;
        float _pitch = 0.0f;
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
                _pitch = GetRandomPitch();
                _soundObject = GetChildSoundEffect("Dust Collect Intro");
                PlaySoundObject(_soundObject, _pitch);
                _length = GetSourceFromObject(_soundObject).clip.length;

                //Queue Loop
                _soundObject = GetChildSoundEffect("Dust Collect Loop");
                GetSourceFromObject(_soundObject).loop = true;
                QueueSoundObject(_soundObject, _length, _pitch);
                break;

            case SFX.Dust_Collect_Stop_Clean:
                //Stop Loop
                _soundObject = GetChildSoundEffect("Dust Collect Loop");
                _source = GetSourceFromObject(_soundObject);
                _source.loop = false;
                _length = _source.clip.length;
                _pitch = _source.pitch;
                _playbackPosition = _source.time;

                //Queue the stop sfx
                _soundObject = GetChildSoundEffect("Dust Collect End");
                QueueSoundObject(_soundObject, _length - _playbackPosition, _pitch);
                break;

            case SFX.Dust_Collect_Stop_Abrupt:
                //Stop loop
                _soundObject = GetChildSoundEffect("Dust Collect Loop");
                _source = GetSourceFromObject(_soundObject);
                _pitch = _source.pitch;
                _source.Stop();

                //Play the stop sfx
                _soundObject = GetChildSoundEffect("Dust Collect End");
                PlaySoundObject(_soundObject, _pitch);
                break;
            case SFX.Foot_Step:
                _soundObject = GetChildSoundEffect("Foot Step");
                // PlaySound(_footstepClips[_stepIndex++ % _footstepClips.Length]);
                PlaySoundObject(_soundObject);
                break;
            case SFX.Land:
                _soundObject = GetChildSoundEffect("Land");
                PlaySoundObject(_soundObject);
                break;
            case SFX.Dash:
                _soundObject = GetChildSoundEffect("Dash");
                PlaySoundObject(_soundObject);
                break;
            case SFX.Dead:
                _soundObject = GetChildSoundEffect("Death");
                PlaySoundObject(_soundObject);
                break;
            case SFX.Rumble:
                _soundObject = GetChildSoundEffect("Rumble");
                PlaySoundObject(_soundObject);
                break;
            case SFX.Rattle:
                _soundObject = GetChildSoundEffect("Rattle");
                PlaySoundObject(_soundObject);
                break;
            default:
                Debug.Log("Requested to play an unimplemented sound effect...");
                break;
        }
    }

    public void PlaySFXByString(string soundEffectName)
    {
        PlaySoundObject(GetChildSoundEffect(soundEffectName));    
    }

    private void PlaySoundObject(GameObject _soundObject)
    {
        QueueSoundObject(_soundObject, 0);
    }

    private void PlaySoundObject(GameObject _soundObject, float pitch)
    {
        QueueSoundObject(_soundObject, 0, pitch);
    }

    private void QueueSoundObject(GameObject _soundObject, float _delay)
    {
        QueueSoundObject(_soundObject, _delay, GetRandomPitch());
    }

    private void QueueSoundObject(GameObject _soundObject, float _delay, float pitch)
    {
        AudioSource _audio = GetSourceFromObject(_soundObject);

        //Queue the requested sound effect
        _audio.pitch = pitch;
        _audio.PlayDelayed(_delay - 0.1f);
    }

    float GetRandomPitch()
    {
        return Random.Range(randomPitchVariationRange.x, randomPitchVariationRange.y);
    }

    //Helper to get an audio source from a game object, with safety checks
    private AudioSource GetSourceFromObject(GameObject _soundObject)
    {
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
        return _soundEffects[_soundEffectsNames.IndexOf(_name)];
    }
}
