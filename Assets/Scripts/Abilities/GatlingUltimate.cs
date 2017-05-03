using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingUltimate : AbstractUltimate
{
    public float activationDuration = 10.0f;

    // hacker has to hold triggers down this amount of time
    public float triggerDownTime = 2.0f;
    private float currentTriggerDownTime;
    private bool triggerDown = false;

    //Verschoben in Oberklasse//private bool activated = false; // if the actual gun has been pulled out

    private void Update()
    {
        // decrease trigger down timer
        if (!activated && triggerDown)
            currentTriggerDownTime -= Time.deltaTime;

        // pressed long enough -> activate && pay price
        // TODO add gesture?
        if (!activated && triggerDown && currentTriggerDownTime <= 0 && TryConsumeDataFragments())
        {
            SwitchActive(true);

            // deactivate after x seconds
            this.Delayed(activationDuration, () => SwitchActive(false));
        }

        // TODO
        if (activated)
        {
            // shoot
        }
    }

    private void LateUpdate()
    {
        // adjust gatling between hands
        if(activated)
        {
            var t = transform.GetChild(0);
            t.position = leftHand.position;
            t.rotation = Quaternion.LookRotation(rightHand.position - leftHand.position);
            t.localScale = new Vector3(0.05f, 0.05f, (rightHand.position - leftHand.position).magnitude * 2);
        }
    }

    protected override void OnTriggerDown()
    {
        // mark trigger down && start timer
        if (!activated)
        {
            triggerDown = true;
            currentTriggerDownTime = triggerDownTime;
        }
    }

    protected override void OnTriggerUp()
    {
        // mark trigger up
        triggerDown = false;
    }

    private void SwitchActive(bool active)
    {
        activated = active;
        transform.GetChild(0).gameObject.SetActive(active);
        triggerDown = active;

        if (active)
            EnableUlti();
        else
            DisableUlti();

        // en/disable old weapons
        // TODO fix and move to AbstractUltimate
        // HackerPlayer hp = hackerPlayer.GetComponentInChildren<HackerPlayer>();
        //hp.GetEquippedAbilityGO(HackerHand.Left).SetActive(!active);
        //hp.GetEquippedAbilityGO(HackerHand.Right).SetActive(!active);
    }

}
