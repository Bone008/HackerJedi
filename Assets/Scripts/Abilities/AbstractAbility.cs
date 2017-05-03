using System;
using UnityEngine;

public abstract class AbstractAbility : MonoBehaviour
{
    public AbilityType abilityType; // to be set by inspector
    public bool needsMirroring;

    /// <summary>The transform of the main player (the head camera of the hacker, also the GO tagged "Player").</summary>
    protected Transform hackerPlayer;

    public AbilityType Type { get { return abilityType; } }

    public bool IsTriggerDown { get; protected set; }
    public bool IsGripDown { get; protected set; }

    /// <summary>Needs to be manually checked by the ability script (if used). Set with CooldownFor(...).</summary>
    public bool IsCoolingDown { get; private set; }

    private Coroutine cooldownCoroutine = null;
    private Action cooldownFinishedCallback = null;


    protected virtual void OnTriggerDown() { }
    protected virtual void OnTriggerUp() { }
    protected virtual void OnGripDown() { }
    protected virtual void OnGripUp() { }


    protected virtual void OnDisable()
    {
        if (cooldownCoroutine != null)
        {
            // abort cooldown
            StopCoroutine(cooldownCoroutine);
            ResetCooldown();
        }
    }


    public void InitHackerPlayer(Transform hackerPlayer)
    {
        this.hackerPlayer = hackerPlayer;
    }

    public void SetTriggerDown(bool value)
    {
        bool wasDown = IsTriggerDown;
        IsTriggerDown = value;

        if (!gameObject.activeSelf)
            return;

        if (value && !wasDown)
            OnTriggerDown();
        else if (!value && wasDown)
            OnTriggerUp();
    }

    public void SetGripDown(bool value)
    {
        bool wasDown = IsGripDown;
        IsGripDown = value;

        if (!gameObject.activeSelf)
            return;

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

    protected void CooldownFor(float time, Action finishedCallback = null)
    {
        if (time > 0)
        {
            IsCoolingDown = true;

            if(cooldownCoroutine != null)
                StopCoroutine(cooldownCoroutine);

            cooldownFinishedCallback = finishedCallback;
            cooldownCoroutine = this.Delayed(time, ResetCooldown);
        }
        else if (finishedCallback != null)
            finishedCallback();
    }

    private void ResetCooldown()
    {
        IsCoolingDown = false;

        if (cooldownFinishedCallback != null)
            cooldownFinishedCallback();

        cooldownCoroutine = null;
        cooldownFinishedCallback = null;
    }

}
