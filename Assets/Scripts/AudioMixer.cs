using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]

public class AudioMixer : MonoBehaviour {

    public float duration = 2.0f;
    public int low = 666;
    public int high = 22000;

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioLowPassFilter alfi = GetComponent<AudioLowPassFilter>();

        if (scene.name == "game")
        {
            // transition from filtered to normal, then deactivate filter
            this.Animate(duration, progress =>
            {
                alfi.cutoffFrequency = Mathf.Lerp(low, high, progress);
            });
            this.Delayed(duration, () => alfi.enabled = false);
        }
        else
        {
            // activate alfi, then transition from normal to filtered
            if (alfi.enabled == false)
            {
                alfi.enabled = true;
                this.Animate(duration, progress =>
                {
                    alfi.cutoffFrequency = Mathf.Lerp(high, low, progress);
                });
            }
        }
    }

}
