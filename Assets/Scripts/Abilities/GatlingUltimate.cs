using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingUltimate : AbstractUltimate
{

    // TODO real gatling ultimate, so far this is only a proof of concept of the system in HackerProgression & HackerPlayer

    private void Update()
    {
        Debug.Log("gatling ult is updating (trigger = " + IsTriggerDown + ")");
    }

    protected override void OnTriggerDown()
    {
        Debug.Log("gatling ult trigger down");
    }

    protected override void OnTriggerUp()
    {
        Debug.Log("gatling ult trigger up");
    }

}
