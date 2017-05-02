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

    protected void EnableUlti()
    {
        hackerPlayer.GetComponent<HackerPlayer>().EnableUlti();
    }
    protected void DisableUlti()
    {
        hackerPlayer.GetComponent<HackerPlayer>().DisableUlti();
    }

}
