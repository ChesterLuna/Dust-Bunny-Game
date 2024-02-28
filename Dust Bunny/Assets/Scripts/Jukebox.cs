using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    // Static members & types
    public static Jukebox instance = null;
    public enum Song{BEDROOM, NONE};
    [Serializable]
    public struct InspectorSong{
        public Song song;
        public AudioClip clip;
    }
    private const string JUKEBOX_PATH = "Jukebox";

    // Song clips
    public InspectorSong[] exposedSongClips;
    private Dictionary<Song, AudioClip> songClips;

    // Variables
    public Song initialSong = Song.NONE;

    [Range(0.0f, 100.0f)]
    public float fadeSpeed = 5.0f;

    // Fade variables
    [SerializeField]
    private Song bgmSwapBuffer;
    [SerializeField]
    private float fadeTimer = 1;
    [SerializeField]
    private bool fadingOut = false;

    // Misc
    private bool initalized = false;

    // References
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        Initalize();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFade();
    }

    public void StartSwapToClip(Song song){
        bgmSwapBuffer = song;
        
        fadeTimer = 100.0f;
        fadingOut = true;

        Debug.Log("Queued to play: " + bgmSwapBuffer);
    }

    public Jukebox Initalize(){
        if (!initalized){
            DontDestroyOnLoad(gameObject);
            instance = this;
            source = GetComponent<AudioSource>();

            //Assemble the disctionary from the inspector songs
            songClips = new Dictionary<Song, AudioClip>();
            for(int i = 0; i < exposedSongClips.Length; i++){
                songClips.Add(exposedSongClips[i].song, exposedSongClips[i].clip);
            }
            songClips.Add(Song.NONE, null);

            //Play the initial song
            bgmSwapBuffer = initialSong;
            SwapClip();

            initalized = true;
        }

        return this;
    }

    private void HandleFade(){
        // Determine which direction to fade the audio
        float fadeDistance = fadeSpeed * Time.deltaTime;
        if(fadingOut) fadeDistance *= -1;
        fadeTimer += fadeDistance;

        // Did we cross the zero boundary
        if(fadeTimer <= 0){
            SwapClip();
        }

        // Clamp to the range 0 - 100
        fadeTimer = Mathf.Max(Mathf.Min(fadeTimer, 100), 0);
        

        source.volume = fadeTimer / 100;
    }

    private void SwapClip(){
        fadingOut = false;
        AudioClip newClip = songClips[bgmSwapBuffer];
        if(newClip != null){
            source.Stop();
            source.clip = newClip;
            source.Play();
        } else {
            source.Stop();
        }

        string name = newClip == null ? "None" : newClip.name;
        Debug.Log("Now Playing: " + name);
    }

    public static void PlaySong(Song song){
        // If there is no jukebox, make a new one
        if(instance == null){
            Instantiate(Resources.Load(JUKEBOX_PATH)).GetComponent<Jukebox>().Initalize();
        }

        instance.StartSwapToClip(song);
    }
}
