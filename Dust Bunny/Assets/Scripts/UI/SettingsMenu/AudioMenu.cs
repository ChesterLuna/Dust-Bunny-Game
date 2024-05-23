using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private AudioMixer _mixer;

    private AudioSource onSFXChangeSFX;
    private bool midSettingUp = false;

    void Start()
    {
        onSFXChangeSFX = GetComponent<AudioSource>();
    }

    public void OnBGMSliderChange()
    {
        _mixer.SetFloat("bgmVolume", Jukebox.RatioToDB(_bgmSlider.value));
    }

    public void OnSFXSliderChange()
    {
        _mixer.SetFloat("sfxVolume", Jukebox.RatioToDB(_sfxSlider.value));
        if (!midSettingUp && (onSFXChangeSFX.time > 0.75 || !onSFXChangeSFX.isPlaying))
        {
            onSFXChangeSFX.Play();
        }
    }

    public void OnEnable()
    {
        midSettingUp = true;
        float volume = 1.0f;
        if (!_mixer.GetFloat("bgmVolume", out volume))
        {
            Debug.LogError("Provided mixer did not have the bgmVolume parameter");
        }
        _bgmSlider.value = Jukebox.DBToRatio(volume);

        if (!_mixer.GetFloat("sfxVolume", out volume))
        {
            Debug.LogError("Provided mixer did not have the sfxVolume parameter");
        }
        _sfxSlider.value = Jukebox.DBToRatio(volume);
        midSettingUp = false;
    }

    public void OnDisable()
    {
        //Write settings to disk
        PlayerPrefs.SetFloat("bgmVolume", _bgmSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", _sfxSlider.value);
    }
}
