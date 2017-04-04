using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackerPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {

        foreach (SteamVR_RenderModel model in GetComponentsInChildren<SteamVR_RenderModel>())
            model.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
