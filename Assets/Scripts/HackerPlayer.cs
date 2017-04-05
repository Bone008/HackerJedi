using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackerPlayer : MonoBehaviour {

	void OnEnable () {
        var controllerInputs = GetComponentsInChildren<SteamVR_TrackedController>();

        foreach(SteamVR_TrackedController controller in controllerInputs)
            controller.TriggerClicked += Controller_TriggerClicked;
    }

    private void OnDisable()
    {
        var controllerInputs = GetComponentsInChildren<SteamVR_TrackedController>();

        foreach (SteamVR_TrackedController controller in controllerInputs)
            controller.TriggerClicked -= Controller_TriggerClicked;
    }

    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        GameObject controllerObj = ((Component)sender).gameObject;

        Gun gun = controllerObj.GetComponentInChildren<Gun>();
        if (gun != null)
            gun.Fire();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
