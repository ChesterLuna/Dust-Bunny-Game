using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Jukebox : MonoBehaviour
{
    // Static members & types
    public static Jukebox instance = null;
    public enum Song{BURROW, BEDROOM, BOSS, TITLE, NONE};
    [Serializable]
    public struct SongInfo{
        public Song song;
        public AudioClip introClip;
        public AudioClip loopClip;
    }
    private const string JUKEBOX_PATH = "Jukebox";

    // Song clips
    public SongInfo[] exposedSongClips;
    private Dictionary<Song, SongInfo> songClips;

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
    private SongInfo currentSong;

    // References
    private AudioSource introSource;
    private AudioSource loopSource;
    [SerializeField] AudioMixer _mixer;

    // Constants
    const float dbMin = -80;
    const float dbMax = 20;


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
            introSource = GetComponents<AudioSource>()[0];
            loopSource = GetComponents<AudioSource>()[1];

            //Assemble the disctionary from the inspector songs
            songClips = new Dictionary<Song, SongInfo>();
            for(int i = 0; i < exposedSongClips.Length; i++){
                songClips.Add(exposedSongClips[i].song, exposedSongClips[i]);
            }
            SongInfo noneSong;
            noneSong.song = Song.NONE; //lol
            noneSong.introClip = null;
            noneSong.loopClip = null;
            songClips.Add(Song.NONE, noneSong);

            //Play the initial song
            bgmSwapBuffer = initialSong;
            SwapClip();

            initalized = true;
        }

        return this;
    }

    private void HandleFade(){
        // Determine which direction to fade the audio
        // Doing this instead of using unscaled time because this way we still run slower during initial lag on game start (desireable, because otherwise the music reaches fade in immediatly)
        // TBH i don't know why this doesnt divide by zero when paused, but it doesn't and it doesn't work if I add a check for it
        float fadeDistance = fadeSpeed * Time.deltaTime / Time.timeScale;
        if(fadingOut) fadeDistance *= -1;
        fadeTimer += fadeDistance;

        // Did we cross the zero boundary
        if(fadeTimer <= 0){
            SwapClip();
        }

        // Clamp to the range 0 - 100
        fadeTimer = Mathf.Max(Mathf.Min(fadeTimer, 100), 0);
        

        introSource.volume = fadeTimer / 100;
        loopSource.volume = fadeTimer / 100;
    }

    private void SwapClip(){
        fadingOut = false;
        currentSong = songClips[bgmSwapBuffer];
        AudioClip newIntroClip = currentSong.introClip;
        AudioClip newLoopClip = currentSong.loopClip;
        if(currentSong.song != Song.NONE){
            introSource.Stop();
            introSource.clip = newIntroClip;

            loopSource.Stop();
            loopSource.clip = newLoopClip;
            
            loopSource.PlayScheduled(AudioSettings.dspTime + newIntroClip.length);
            introSource.Play();
        } else {
            introSource.Stop();
            loopSource.Stop();
        }

        string name = newIntroClip == null ? "None" : newIntroClip.name;
        Debug.Log("Now Playing: " + name);
    }

    public static void PlaySong(Song song){
        // If there is no jukebox, make a new one
        if(instance == null){
            Instantiate(Resources.Load(JUKEBOX_PATH)).GetComponent<Jukebox>().Initalize();

            //Also load the audio settings from disk
            instance._mixer.SetFloat("bgmVolume", RatioToDB(PlayerPrefs.GetFloat("bgmVolume")));
            instance._mixer.SetFloat("sfxVolume", RatioToDB(PlayerPrefs.GetFloat("sfxVolume")));
        }

        // If this song is also the currently playing song, do nothing
        if(song == instance.currentSong.song){
            return;
        }

        instance.StartSwapToClip(song);
    }

    //Utility Functions
    public static float RatioToDB(float ratio){
        return Mathf.Lerp(dbMin, dbMax, ratio);
    }

    public static float DBToRatio(float db){
        return (db - dbMin) / (dbMax - dbMin);
    }
}
