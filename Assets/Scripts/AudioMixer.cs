using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]

public class AudioMixer : MonoBehaviour {
    
	
	// Update is called once per frame
	void Update () {
    
        
	}

    private void OnLevelWasLoaded(int level)
    {
        if (level == 2)
        {
            GetComponent<AudioLowPassFilter>().cutoffFrequency = 666f;
        }
        if (level == 1)
        {
            GetComponent<AudioLowPassFilter>().cutoffFrequency = 5000f;
        }
    }
}
