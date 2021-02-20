
using System.Collections.Generic;
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
            s.source.spatialBlend = 1.0f;
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

        AudioSource.PlayClipAtPoint(s.clip, position);
    }

    public static void PlaySound(string name, Vector3 position) {
        instance.Play(name, position);
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

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
