using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    
    public static SoundManager instance;

    public AudioClip[] musicClips;
    public AudioClip[] soundClips;

    public AudioSource musicSource;
    public AudioSource soundEffectsSource;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } 
        else {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        musicSource.clip = musicClips[0];
        musicSource.Play();
    }
}
