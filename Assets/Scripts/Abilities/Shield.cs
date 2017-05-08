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
    public GameObject shieldTransform;


    private void Start()
    {
        isActive = false;
        isRecovering = false;
        shieldTransform.transform.localScale = new Vector3(0, 0, 0);
    }


    private void Update()
    {
        if (isActive)
        {
            energy -= drainRate * Time.deltaTime;

            if(energy <= 0)
            {
                isActive = false;
                isRecovering = true;
                this.AnimateVector(0.1f, new Vector3(0.15f, 0.15f, 0.15f), Vector3.zero, Util.EaseInOut01, v => shieldTransform.transform.localScale = v);
                isActive = false;
            }
        }

    }

    protected override void OnTriggerDown()
    {
        if (!isRecovering)
        {
            this.AnimateVector(0.1f, Vector3.zero, new Vector3(0.15f, 0.15f, 0.15f), Util.EaseInOut01, v => shieldTransform.transform.localScale = v);
            isActive = true;
        }
    }

    protected override void OnTriggerUp()
    {
        this.AnimateVector(0.1f,new Vector3(0.15f, 0.15f, 0.15f), Vector3.zero, Util.EaseInOut01, v => shieldTransform.transform.localScale = v);
        isActive = false;
    }
}
