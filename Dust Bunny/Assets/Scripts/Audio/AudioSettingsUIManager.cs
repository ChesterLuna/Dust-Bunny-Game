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

    const float dbMin = -80;
    const float dbMax = 20;
    // Start is called before the first frame update
    void Start()
    {
        float volume = 1.0f;
        if (!_mixer.GetFloat("bgmVolume", out volume)){
            Debug.LogError("Provided mixer did not have the bgmVolume parameter");
        }
        _bgmSlider.value = DBToRatio(volume);

        if (!_mixer.GetFloat("sfxVolume", out volume)){
            Debug.LogError("Provided mixer did not have the sfxVolume parameter");
        }
        _sfxSlider.value = DBToRatio(volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBGMSliderChange(){
        _mixer.SetFloat("bgmVolume", RatioToDB(_bgmSlider.value));
    }

    public void OnSFXSliderChange(){
        _mixer.SetFloat("sfxVolume", RatioToDB(_sfxSlider.value));
    }

    public float RatioToDB(float ratio){
        return Mathf.Lerp(dbMin, dbMax, ratio);
    }

    public float DBToRatio(float db){
        return (db - dbMin) / (dbMax - dbMin);
    }
}
