using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedController))]
public class ControllerInputHandler : MonoBehaviour {

    public HackerPlayer player;
    public HackerHand hand;
    
    private SteamVR_TrackedController controller;

    void Awake()
    {
        controller = GetComponent<SteamVR_TrackedController>();
    }

    void OnEnable()
    {
        controller.TriggerClicked += Controller_TriggerClicked;
        controller.PadClicked += Controller_PadClicked;
        controller.PadTouched += Controller_PadTouched;
        controller.PadUntouched += Controller_PadUntouched;

        Debug.Log("now using controller " + controller.controllerIndex);
    }

    private void OnDisable()
    {
        controller.TriggerClicked -= Controller_TriggerClicked;
        controller.PadClicked -= Controller_PadClicked;
        controller.PadTouched -= Controller_PadTouched;
        controller.PadUntouched -= Controller_PadUntouched;
        Debug.Log("no longer using controller " + controller.controllerIndex);
    }

    private HackerHand GetCurrentHand()
    {
        return hand;

        // note: this doesn't really help, because we explicitly bind the left/right role to the game objects
        // so dynamic switching only messes stuff up

        //if (SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost) == controller.controllerIndex)
        //    return HackerHand.Left;
        //if (SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost) == controller.controllerIndex)
        //    return HackerHand.Right;

        //Debug.Log("no idea which hand i am! my index: " + controller.controllerIndex, this.gameObject);
        //return HackerHand.Right; // return right by default
    }

    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        player.Fire(GetCurrentHand());
    }

    private void Controller_PadTouched(object sender, ClickedEventArgs e)
    {
        player.OpenAbilitySelectionWheel(GetCurrentHand());
    }

    private void Controller_PadUntouched(object sender, ClickedEventArgs e)
    {
        player.CloseAbilitySelectionWheel(GetCurrentHand());
    }


    private void Controller_PadClicked(object sender, ClickedEventArgs e)
    {
        player.ConfirmAbilitySelection(GetCurrentHand());
    }


}
