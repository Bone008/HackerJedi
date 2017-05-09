using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(SteamVR_TrackedObject), typeof(SteamVR_TrackedController))]
public class ControllerInputHandler : MonoBehaviour {

    public HackerPlayer player;
    public HackerHand hand;

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_TrackedController trackedController;


    // for haptic feedback
    private float hapticStrength = 0;
    private float hapticRemainingDuration = 0;

    private SteamVR_Controller.Device GetController()
    {
        return SteamVR_Controller.Input((int)trackedObject.index);
    }

    void Awake()
    {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        trackedController = GetComponent<SteamVR_TrackedController>();
    }

    void OnEnable()
    {
        player.HapticFeedback += Player_HapticFeedback;
        trackedController.MenuButtonClicked += Controller_MenuButtonClicked;
        trackedController.TriggerClicked += Controller_TriggerClicked;
        trackedController.TriggerUnclicked += Controller_TriggerUnclicked;
        trackedController.Gripped += Controller_Gripped;
        trackedController.Ungripped += Controller_Ungripped;
        trackedController.PadClicked += Controller_PadClicked;
        trackedController.PadTouched += Controller_PadTouched;
        trackedController.PadUntouched += Controller_PadUntouched;

        Debug.Log("now using controller " + trackedObject.index);
    }

    void OnDisable()
    {
        player.HapticFeedback -= Player_HapticFeedback;

        trackedController.TriggerClicked -= Controller_TriggerClicked;
        trackedController.TriggerUnclicked -= Controller_TriggerUnclicked;
        trackedController.Gripped -= Controller_Gripped;
        trackedController.Ungripped -= Controller_Ungripped;
        trackedController.PadClicked -= Controller_PadClicked;
        trackedController.PadTouched -= Controller_PadTouched;
        trackedController.PadUntouched -= Controller_PadUntouched;

        Debug.Log("no longer using controller " + trackedObject.index);
    }

    void Update()
    {
        SteamVR_Controller.Device controller = GetController();
        if(trackedController.padTouched)
        {
            Vector2 touchPosition = controller.GetAxis();
            player.SetAbilitySelectionPosition(hand, touchPosition);
        }

        if(hapticRemainingDuration > 0)
        {
            hapticRemainingDuration -= Time.unscaledDeltaTime;

            // trigger the pulse for each frame for the duration
            ushort nativeStrength = (ushort)(Mathf.Clamp01(hapticStrength) * 3999);
            controller.TriggerHapticPulse(nativeStrength);
        }
    }

    // ==== output (haptics) ====

    private void Player_HapticFeedback(HackerHand hand, float strength, float duration)
    {
        if (hand != GetCurrentHand())
            return;
        
        hapticRemainingDuration = duration;
        hapticStrength = strength;
    }



    // ==== input ====

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

}
