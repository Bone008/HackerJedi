using MKGlowSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MasterEye : MonoBehaviour {

    public float signalLostDuration = 5.0f;
    public GameObject deathExplosion;
    public LayerMask explosionLayer;

    public VideoPlayer signalLostPlayer;
    public GameObject signalLostBackground;

    public Camera masterCamera;
    private MKGlow mkGlow;

    private void Start()
    {
        mkGlow = masterCamera.GetComponent<MKGlow>();
    }

    public void OnDeath(HealthResource health)
    {
        // explosion only visible to hacker
        GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        explosion.layer = explosionLayer.value;

        // start video, enable black background, disable glow, disable health script (-> only 1 ondeath)
        signalLostPlayer.Play();
        signalLostBackground.SetActive(true);
        mkGlow.enabled = false;
        health.enabled = false;

        this.Delayed(signalLostDuration, () => 
        {
            // stop video, disable background, enable glow and health again
            signalLostPlayer.Stop();
            signalLostBackground.SetActive(false);
            mkGlow.enabled = true;
            health.enabled = true;

            health.RestoreFullHealth();
        });
    }
    
}
