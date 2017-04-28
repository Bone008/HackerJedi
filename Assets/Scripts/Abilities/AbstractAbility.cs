using System;
using UnityEngine;

public abstract class AbstractAbility : MonoBehaviour
{
    public abstract AbilityType Type { get; }

    protected bool isTriggerDown;
    protected bool isGripDown;

    protected virtual void OnTriggerDown() { }
    protected virtual void OnTriggerUp() { }
    protected virtual void OnGripDown() { }
    protected virtual void OnGripUp() { }

    public void SetTriggerDown(bool value)
    {
        bool wasDown = isTriggerDown;
        isTriggerDown = value;

        if (value && !wasDown)
            OnTriggerDown();
        else if (!value && wasDown)
            OnTriggerUp();
    }

    public void SetGripDown(bool value)
    {
        bool wasDown = isGripDown;
        isGripDown = value;

        if (value && !wasDown)
            OnGripDown();
        else if (!value && wasDown)
            OnGripUp();
    }

    /// <summary>Simulates pressing and releasing the trigger instantaneously.</summary>
    public void FireOnce()
    {
        SetTriggerDown(true);
        SetTriggerDown(false);
    }


    /// <summary>Utility function for ability scripts to get a ray in the direction the hand is aiming</summary>
    public Ray GetAimRay(Transform nozzle = null)
    {
        Transform trans = nozzle ?? transform; // use own transform if no nozzle is defined

        var aimDirection = trans.TransformDirection(Vector3.forward).normalized;
        return new Ray(trans.position, aimDirection);
    }

    //returns if trigger is pressed
    public bool getTriggerInfo()
    {
        return isTriggerDown;
    }

    //returns if grip is pressed
    public bool getGripInfo()
    {
        return isGripDown;
    }
}
