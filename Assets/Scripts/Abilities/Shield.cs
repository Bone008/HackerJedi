using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : AbstractAbility
{

    public float energy;
    public float drainRate;
    public float cooldown;
    private bool isActive;
    private bool isRecovering;


    private void Start()
    {
        isActive = false;
        isRecovering = false;
    }


    private void Update()
    {
        if (isActive)
        {
            energy -= drainRate * Time.deltaTime;
        }

    }

    protected override void OnTriggerDown()
    {
        base.OnTriggerDown();

    }
}
