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

        signalLostPlayer.Prepare();
    }

    public void OnDeath(HealthResource health)
    {
        // explosion only visible to hacker
        GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        explosion.layer = LayerMask.NameToLayer("NotVisibleToMaster");

        // start video, enable overlay with render texture, disable glow, disable health script (-> only 1 ondeath)
        signalLostPlayer.Play();
        signalLostBackground.SetActive(true);
        health.enabled = false;

        // has to be at end of frame because OnDeath gets called in OnTriggerEnter
        // and disabling MKGlow destroys some GameObjects and that is not allowed in Collision functions
        this.Delayed(new WaitForEndOfFrame(), () => mkGlow.enabled = false);

        this.Delayed(signalLostDuration, () => 
        {
            // stop video, disable overlay, enable glow and health again
            signalLostPlayer.Pause();
            signalLostBackground.SetActive(false);
            health.enabled = true;
            health.RestoreFullHealth();

            mkGlow.enabled = true;
        });
    }

}
