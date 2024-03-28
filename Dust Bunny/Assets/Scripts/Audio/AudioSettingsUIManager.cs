using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUIManager : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private AudioMixer _mixer;

    public void OnBGMSliderChange(){
        _mixer.SetFloat("bgmVolume", Jukebox.RatioToDB(_bgmSlider.value));
    }

    public void OnSFXSliderChange(){
        _mixer.SetFloat("sfxVolume", Jukebox.RatioToDB(_sfxSlider.value));
    }

    public void OnEnable()
    {
        float volume = 1.0f;
        if (!_mixer.GetFloat("bgmVolume", out volume)){
            Debug.LogError("Provided mixer did not have the bgmVolume parameter");
        }
        _bgmSlider.value = Jukebox.DBToRatio(volume);

        if (!_mixer.GetFloat("sfxVolume", out volume)){
            Debug.LogError("Provided mixer did not have the sfxVolume parameter");
        }
        _sfxSlider.value = Jukebox.DBToRatio(volume);
    }

    public void OnDisable(){
        //Write settings to disk
        PlayerPrefs.SetFloat("bgmVolume", _bgmSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", _sfxSlider.value);
    }
}
