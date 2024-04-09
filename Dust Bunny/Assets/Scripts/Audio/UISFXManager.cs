using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class UISFXManager : MonoBehaviour
{
    public enum SFX{POSITIVE, NEGATIVE, NAVIGATE};

    [Serializable]
    public struct SFXInfo{
        public SFX sfx;
        public AudioClip clip;
    }
    private const string UISFX_PATH = "UI/UI SFX Manager";

    public static UISFXManager instance;

    // SFX clips
    public SFXInfo[] exposedSFX;
    private Dictionary<SFX, SFXInfo> sfxClips;
    private AudioSource[] sources;
    private bool initalized = false;

    // Start is called before the first frame update
    void Start()
    {
        Initalize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public UISFXManager Initalize(){
        if (!initalized){
            DontDestroyOnLoad(gameObject);
            instance = this;
            sources = GetComponents<AudioSource>();

            //Assemble the disctionary from the inspector songs
            sfxClips = new Dictionary<SFX, SFXInfo>();
            for(int i = 0; i < exposedSFX.Length; i++){
                sfxClips.Add(exposedSFX[i].sfx, exposedSFX[i]);
            }

            initalized = true;
        }

        return this;
    }

    // Linear search for the oldest playing sound effect slot to play over, and then play the sfx on that source
    private void FindSourceToPlay(SFX sfx){
        AudioSource mostStaleSource = sources[0];
        for(int i = 0; i < sources.Length; i++){
            if(mostStaleSource.isPlaying && sources[i].time > mostStaleSource.time){
                mostStaleSource = sources[i];
            }
        }

        mostStaleSource.clip = sfxClips[sfx].clip;
        mostStaleSource.Play();
    }

    public static void PlaySFX(SFX sfx){
        // If there is no jukebox, make a new one
        if(instance == null){
            Instantiate(Resources.Load(UISFX_PATH)).GetComponent<UISFXManager>().Initalize();
        }

        Debug.Log("Playing sfx: " + sfx.ToString());

        instance.FindSourceToPlay(sfx);
    }
}
