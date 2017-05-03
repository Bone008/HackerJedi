using MKGlowSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MasterEye : MonoBehaviour {

    public float signalLostDuration = 5.0f;
    public GameObject deathExplosion;

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
        explosion.layer = LayerMask.NameToLayer("NotVisibleToMaster");

        // start video, enable black background, disable glow, disable health script (-> only 1 ondeath)
        signalLostPlayer.Play();
        signalLostBackground.SetActive(true);
        StartCoroutine(StopGlow());
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

    private IEnumerator StopGlow()
    {
        // has to be here because OnDeath gets called in OnTriggerEnter
        // and disabling MKGlow destroys some GameObjects and that is not allowed in Collision functions
        yield return new WaitForEndOfFrame();
        mkGlow.enabled = false;
    }

}
