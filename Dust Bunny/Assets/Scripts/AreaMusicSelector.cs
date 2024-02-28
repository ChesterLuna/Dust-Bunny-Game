using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMusicSelector : MonoBehaviour
{
    public Jukebox.Song song;
    // Start is called before the first frame update
    void Start()
    {
        Jukebox.PlaySong(song);
        Destroy(gameObject);
    }
}
