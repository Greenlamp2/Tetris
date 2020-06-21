using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;
    public AudioClip lineClearedSound;

    public AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
}