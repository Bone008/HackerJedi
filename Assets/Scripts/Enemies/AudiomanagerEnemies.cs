using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudiomanagerEnemies : MonoBehaviour {

    // Array with soundsclips for enemies; as of now, only Grunt
    public AudioClip[] enemyBirthAudio;
    public AudioClip[] enemyDeathAudio;
    public new AudioSource audio;
    public int birthMin;
    public int birthMax;
    public int deathMin;
    public int deathMax;
    private int rnd;

    // Use this for initialization
    void Start()
    {
        sayHello();
        HealthResource health = GetComponent<HealthResource>();
        health.onDeath.AddListener(sayGoodbye);
    }

        // Update is called once per frame
    void Update() {
        
      
    }
    
    // Plays sound when enemy appears
    private void sayHello() 
    {
        rnd = Random.Range(birthMin, birthMax);
        if (rnd < enemyBirthAudio.Length)
        {
            audio.clip = enemyBirthAudio[rnd];
            audio.Play();
        }
    }
    //Plays sound when enemy dies
    private void sayGoodbye()
    {
        GameObject soundObj = new GameObject("Enemy Dying");
        soundObj.transform.position = transform.position;
        AudioSource sound = soundObj.AddComponent<AudioSource>() as AudioSource;
        soundObj.AddComponent<KillAudioObject>();

        rnd = Random.Range(deathMin, deathMax);
        if (rnd < enemyDeathAudio.Length)
        {
            sound.clip = enemyDeathAudio[rnd];
            sound.Play();
        }
    }
     
}
