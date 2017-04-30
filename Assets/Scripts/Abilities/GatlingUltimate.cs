using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingUltimate : AbstractUltimate
{
    // TODO real gatling ultimate, so far this is only a proof of concept of the system in HackerProgression & HackerPlayer

    public float activationDuration = 10.0f;

    private bool activated = false; // if the actual gun has been pulled out

    private void Update()
    {
        Debug.Log("gatling ult is updating (trigger = " + IsTriggerDown + ")");
    }

    private void LateUpdate()
    {
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
        Debug.Log("gatling ult trigger down");

        // initially: activate && pay price
        if (!activated && TryConsumeDataFragments())
        {
            activated = true;
            transform.GetChild(0).gameObject.SetActive(true);

            // deactivate after x seconds
            this.Delayed(activationDuration, () =>
            {
                activated = false;
                transform.GetChild(0).gameObject.SetActive(false);
            });
        }
    }

    protected override void OnTriggerUp()
    {
        Debug.Log("gatling ult trigger up");
    }

}
