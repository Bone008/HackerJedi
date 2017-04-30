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

}
