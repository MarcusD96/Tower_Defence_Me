
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public Sound[] music;
    public Sound currentSong;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake() {
        if(instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        InitializeSounds(sounds, Settings.Sounds, 1.0f);

        InitializeSounds(music, Settings.Music, 0.0f);
        musicNum = Random.Range(0, music.Length - 1); //random starting position in list of songs
        currentSong = music[musicNum];
    }

    public static bool Main = false;
    private void LateUpdate() {
        if(Main) {
            StartCoroutine(BackgroundMusic());
            Main = false;
        }
    }

    void InitializeSounds(Sound[] sounds, float volumeScale, float spacial) {
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.volume *= volumeScale;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = spacial;
        }
    }

    public void UpdateVolume() {
        foreach(Sound s in music) {
            s.source.volume = s.volume = Settings.Music;
        }
        foreach(Sound s in sounds) {
            s.source.volume = s.volume = Settings.Sounds;
        }
    }

    public void Stop(string name) {
        Sound s = System.Array.Find(sounds, sounds => sounds.name == name);
        if(s == null) {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    public static void StaticStop(string name) {
        instance.Stop(name);
    }

    public static void StaticStopAllSounds() {
        foreach(var s in instance.sounds) {
            instance.Stop(s.name);
        }
    }

    public void Play(string name, Vector3 position) {
        Sound s = System.Array.Find(sounds, sounds => sounds.name == name);
        if(s == null) {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        AudioSource.PlayClipAtPoint(s.source.clip, position, s.source.volume);
    }

    public static void StaticPlay(string name, Vector3 position) {
        instance.Play(name, position);
    }

    public int musicNum;
    IEnumerator BackgroundMusic() {
        currentSong = music[musicNum];
        currentSong.source.PlayOneShot(currentSong.clip);

        musicNum++;
        if(musicNum > music.Length - 1)
            musicNum = 0;

        yield return new WaitForSecondsRealtime(currentSong.clip.length);

        StartCoroutine(BackgroundMusic());
    }

    public void NextSong() {
        currentSong.source.Stop();
        StopAllCoroutines();
        StartCoroutine(BackgroundMusic());
    }

    public static void StaticNextSong() {
        instance.NextSong();
    }

    public string GetSongName() {
        return currentSong.name;
    }

    public static string StaticGetSongName() {
        return instance.GetSongName();
    }
}


[System.Serializable]
public class Sound {

    public string name;

    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float volume;
    [Range(0.1f, 3.0f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
