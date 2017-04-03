using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        var deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        if (deviceIndex != -1 && SteamVR_Controller.Input(deviceIndex).GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("test");
            SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(1200);
        }


        deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        if (deviceIndex != -1 && SteamVR_Controller.Input(deviceIndex).GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("test r");
            SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(1000);
        }

    }
}
