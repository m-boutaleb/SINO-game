﻿using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    // This method is called just before the start method
    void Awake() {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.clip = s.clip;

            s.source.loop = s.loop;
        }
    }

    public void Play(string name) {
        Sound sourceFounded = Array.Find(sounds, sound => sound.name == name);

        // Manage source not found
        if (sourceFounded == null) {
            Debug.LogWarning("Error: sound with name: '" + name + "' has not been founded");
            return;
        }

        // To make a more realistic sound each sound cannot sound with the same volume and pitch
        if (name == "FootStepsSound") {
            sourceFounded.volume = UnityEngine.Random.Range(0.8f, 1f);
            sourceFounded.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
        }

        sourceFounded.source.Play();
    }


    public bool isPlaying(string name) {
        Sound sourceFounded = Array.Find(sounds, sound => sound.name == name);

        // Manage source not found
        if (sourceFounded == null) {
            Debug.LogWarning("Error: sound with name: '" + name + "' has not been founded");
            return false;
        }
        return sourceFounded.source.isPlaying;
    }

    public void Stop(string name) {
        Sound sourceFounded = Array.Find(sounds, sound => sound.name == name);

        // Manage source not found
        if (sourceFounded == null) {
            Debug.LogWarning("Error: sound with name: '" + name + "' has not been founded");
            return;
        }

        sourceFounded.source.Stop();
    }
}