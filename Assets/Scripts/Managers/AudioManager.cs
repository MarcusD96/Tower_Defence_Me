
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public Sound[] music;
    public AudioSource currentSong;

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
        music = Shuffle(music);
        currentSong = music[0].source;
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
            s.source.volume = s.volume * volumeScale;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = spacial;
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

    public static void StopSound(string name) {
        instance.Stop(name);
    }

    public static void StopAllSounds() {
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
        AudioSource.PlayClipAtPoint(s.source.clip, position, s.source.volume * Settings.Sounds);
    }

    public static void PlaySound(string name, Vector3 position) {
        instance.Play(name, position);
    }

    Sound[] Shuffle(Sound[] list) {
        int n = list.Length;
        System.Random rng = new System.Random();
        while(n > 1) {
            n--;
            int k = rng.Next(n + 1);
            Sound value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    IEnumerator BackgroundMusic() {
        //play the song, then the next once prev song finishes
        for(int i = 0; i < music.Length; i++) {
            music[i].source.PlayOneShot(music[i].clip, Settings.Music);

            currentSong = music[i].source;
            yield return new WaitForSecondsRealtime(music[i].clip.length);
        }
        //once all songs are complete, start new coroutine and finish current one
        StartCoroutine(BackgroundMusic());
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

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}
