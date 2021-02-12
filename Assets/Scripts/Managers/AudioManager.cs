
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

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

        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Stop(Sound s) {
        if(s == null) {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    public static void StopSound(Sound s) {
        instance.Stop(s);
    }

    public Sound Play(string name) {
        Sound s = System.Array.Find(sounds, sounds => sounds.name == name);
        if(s == null) {
            Debug.LogWarning("Sound: " + name + " not found");
            return null;
        }

        s.source.Play();
        return s;
    }

    public static Sound PlaySound(string name) {
        return instance.Play(name);
    }
}


[System.Serializable]
public class Sound {

    public string name;

    public AudioClip clip;

    [Range(0.0f, 0.1f)]
    public float volume;
    [Range(0.1f, 3.0f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
