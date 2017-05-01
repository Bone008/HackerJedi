using System;
using UnityEngine;

/// <summary>
/// An ultimate script has functionality like a normal ability script.
/// However, because it deals with both hands at the same time, "trigger down" means "both triggers down" (same with grip)
/// and there are explicit references to the transforms of the left and right hand.
/// </summary>
public abstract class AbstractUltimate : AbstractAbility
{
    public float dataFragmentsCost = 5.0f; // can be changed in the inspector

    protected Transform leftHand;
    protected Transform rightHand;


    public void InitHands(Transform leftHand, Transform rightHand)
    {
        this.leftHand = leftHand;
        this.rightHand = rightHand;
    }


    /// <summary>
    /// Attempts to consume <code>dataFragmentsCost</code> from the hacker's resources.
    /// </summary>
    /// <returns>true if successful</returns>
    protected bool TryConsumeDataFragments()
    {
        return hackerPlayer.GetComponent<DataFragmentResource>().SafeChangeValue(-dataFragmentsCost);
    }

    /// <summary>
    /// Attempts to consume a specified amount of data fragments from the hacker's resources.
    /// </summary>
    /// <returns>true if successful</returns>
    protected bool TryConsumeDataFragments(float amount)
    {
        return hackerPlayer.GetComponent<DataFragmentResource>().SafeChangeValue(-amount);
    }
protected bool bothTriggersDown;
    protected bool bothGripsDown;

    protected virtual void OnTriggersDown(Vector3 handLeft, Vector3 handRight) { }
    protected virtual void OnTriggerUp(Vector3 handLeft, Vector3 handRight) { }
    protected virtual void OnGripsDown(Vector3 handLeft, Vector3 handRight) { }
    protected virtual void OnGripUp(Vector3 handLeft, Vector3 handRight) { }
    //Checks if the positions of the controllers are possible Move-Startpoints
    public virtual bool CheckForStartpoint(Transform handLeft, Transform handRight) { return false; }

    //Should only be called when both triggers are pressed (or one released) and the controllers are at the correct position (-> CheckForStartpoint())
    public bool SetTriggersDown(bool value, Transform handLeft, Transform handRight)
    {
        bool wasDown = bothTriggersDown;
        //bothTriggersDown = value;

        if (value && !wasDown)
        {
            if (CheckForStartpoint(handLeft, handRight))
            {
                OnTriggersDown(handLeft.position, handRight.position);
                bothTriggersDown = value;
                return true;
            }
        }
        else if (!value && wasDown)
        {
            OnTriggerUp(handLeft.position, handRight.position);//Hier muss eventuell noch gecheckt werden, ob der Move auch correct war         !!!!!
            bothTriggersDown = value;
            return true;
        }
        return false;
    }

    //Should only be called when both grips are pressed (or one released) and the controllers are at the correct position (-> CheckForStartpoint())
    public bool SetGripsDown(bool value, Transform handLeft, Transform handRight)
    {

        bool wasDown = bothGripsDown;
        //bothGripsDown = value;(darf nur bei einem Wechsel gesetzt werden)

        if (value && !wasDown)
        {
            if (CheckForStartpoint(handLeft, handRight))
            {
                OnGripsDown(handLeft.position,handRight.position);
                bothGripsDown = value;
                return true;
            }
        }
        else if (!value && wasDown)
        {
            OnGripUp(handLeft.position, handRight.position);//Hier muss eventuell noch gecheckt werden, ob der Move auch correct war         !!!!!
            bothGripsDown = value;
            return true;
        }
        return false;
    }

    /// <summary>Simulates pressing and releasing the trigger instantaneously.</summary>
    /*public void FireOnce()
    {
        SetTriggersDown(true);
        SetTriggersDown(false);
    }*/


    /// <summary>Utility function for ability scripts to get a ray in the direction the hand is aiming</summary>
    public Ray GetAimRay(Transform nozzle = null)
    {
        Transform trans = nozzle ?? transform; // use own transform if no nozzle is defined

        var aimDirection = trans.TransformDirection(Vector3.forward).normalized;
        return new Ray(trans.position, aimDirection);
    }
}
/*Ablauf:
 * 1. HackerPlayer registriert Drücken von Trigger/Grip                     (Input -> HackerPlayer)
 * 2. HackerPlayer ermittelt, ob Ultimate aufgeladen und möglich ist        (HackerPlayer)
 * 3.1 Falls ja: HackerPlayer fragt HandPos ab und schickt es an Ultimate   (Ultimate: SetTriggers/Grips)
 * 3.2 Falls Nein: HackerPlayer setzt normalen Move um                      (Ability)
 * 4. Ultimate Checkt, ob Position und Rotation stimmen                     (Ultimate: SetTrigger/Grips -> CheckPosRot)
 * 5.1 Stimmt: Return true und ggf OnTriggerDown                            (SetTrigger ~> HackerPlayer)
 * 5.2 Nicht: Return false, Hackerplayer bricht Ultimate ab                 (SetTrigger -> HackerPlayer)
 * 6. On...Down speichert Positionen                                        (On...Down)
 * 7. Bei Loslassen eines Buttons: Check auf Ultimateabbruch                (
 */
