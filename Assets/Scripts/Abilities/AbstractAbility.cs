using System;
using UnityEngine;

public abstract class AbstractAbility : MonoBehaviour
{
    public abstract AbilityType Type { get; }
    
    protected bool isTriggerDown;

    protected virtual void OnTriggerDown() { }
    protected virtual void OnTriggerUp() { }

    public void SetTriggerDown(bool value)
    {
        try
        {
            if (value && !isTriggerDown)
                OnTriggerDown();
            else if (!value && isTriggerDown)
                OnTriggerUp();
        }
        finally
        {
            isTriggerDown = value;
        }
    }

    /// <summary>Simulates pressing and releasing the trigger instantaneously.</summary>
    public void FireOnce()
    {
        SetTriggerDown(true);
        SetTriggerDown(false);
    }
}
