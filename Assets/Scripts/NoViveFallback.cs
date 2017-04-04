using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NoViveFallback : MonoBehaviour {

    public Transform headCamera;

	// Use this for initialization
	void Awake () {
        if (SteamVR.instance.hmd_TrackingSystemName.Contains("oculus"))
        {
            // move up so the camera doesn't spawn on the floor
            transform.localPosition += Vector3.up * 1.0f;

            // attach gun to head instead of controllers
            var gun = GetComponentInChildren<Gun>();
            if(gun != null)
            {
                gun.transform.SetParent(headCamera, false);
                // move to a position in view slightly to the right
                gun.transform.localPosition = new Vector3(0.1f, -0.169f, 0.402f);
                gun.transform.localEulerAngles = new Vector3(14, 0, 0);
            }
        }

    }

    // Update is called once per frame
    void Update () {
        
    }
}
