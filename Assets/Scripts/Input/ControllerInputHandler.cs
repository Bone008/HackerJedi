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
        controller.MenuButtonClicked += Controller_MenuButtonClicked;
        controller.TriggerClicked += Controller_TriggerClicked;
        controller.TriggerUnclicked += Controller_TriggerUnclicked;
        controller.Gripped += Controller_Gripped;
        controller.Ungripped += Controller_Ungripped;
        controller.PadClicked += Controller_PadClicked;
        controller.PadTouched += Controller_PadTouched;
        controller.PadUntouched += Controller_PadUntouched;

        Debug.Log("now using controller " + controller.controllerIndex);
    }

    void OnDisable()
    {
        controller.TriggerClicked -= Controller_TriggerClicked;
        controller.TriggerUnclicked -= Controller_TriggerUnclicked;
        controller.Gripped -= Controller_Gripped;
        controller.Ungripped -= Controller_Ungripped;
        controller.PadClicked -= Controller_PadClicked;
        controller.PadTouched -= Controller_PadTouched;
        controller.PadUntouched -= Controller_PadUntouched;
        Debug.Log("no longer using controller " + controller.controllerIndex);
    }

    void Update()
    {
        if(controller.padTouched)
        {
            Vector2 touchPosition = SteamVR_Controller.Input((int)controller.controllerIndex).GetAxis();
            player.SetAbilitySelectionPosition(hand, touchPosition);
        }
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

    private void Controller_MenuButtonClicked(object sender, ClickedEventArgs e)
    {
        GameData.Instance.hackerIsReady = true;
    }

    private void Controller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        player.SetTriggerDown(GetCurrentHand(), true);
    }

    private void Controller_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        player.SetTriggerDown(GetCurrentHand(), false);
    }

    private void Controller_Gripped(object sender, ClickedEventArgs e)
    {
        player.SetGripDown(GetCurrentHand(), true);
    }

    private void Controller_Ungripped(object sender, ClickedEventArgs e)
    {
        player.SetGripDown(GetCurrentHand(), false);
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
        // make sure it is open (sometimes, clicked may be fired before touched)
        player.OpenAbilitySelectionWheel(GetCurrentHand());
        player.ConfirmAbilitySelection(GetCurrentHand(), new Vector2(e.padX, e.padY));
    }


}
