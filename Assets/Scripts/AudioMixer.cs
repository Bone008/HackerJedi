using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]

public class AudioMixer : MonoBehaviour {

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "game")
        {
            GetComponent<AudioLowPassFilter>().enabled = false;
        }
        else
        {
            GetComponent<AudioLowPassFilter>().enabled = true;
        }
    }

}
