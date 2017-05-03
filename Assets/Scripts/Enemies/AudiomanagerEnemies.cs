using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiomanagerEnemies : MonoBehaviour {

    public AudioClip[] audioClips;
    public AudioSource _audio;
    // Use this for initialization
    void Start () {
        int rnd = Random.Range(0, 5);
        if (rnd < audioClips.Length) {
            _audio.clip = audioClips[rnd];
            _audio.Play();
        }
        
	}
	
	// Update is called once per frame
	void Update () {

	}
}
