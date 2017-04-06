using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;

//for throwing the enemy
[RequireComponent(typeof(VelocityEstimator))]
[RequireComponent(typeof(Rigidbody))]
//end
public class Throwable_OBJ : MonoBehaviour
{

    private VelocityEstimator velocityEstimator; //for throwing the enemy
    private new Rigidbody rigidbody;

    void Awake()
    {
        velocityEstimator = GetComponent<VelocityEstimator>();
        rigidbody = GetComponent<Rigidbody>();
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Throwing-Functionality
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void setGrabbed()
    {
        velocityEstimator.BeginEstimatingVelocity();
        rigidbody.useGravity = false;
        //evtl bool grabed? For Movementdeaktivation (grabed=true)
    }

    public void setFree()
    {
        velocityEstimator.FinishEstimatingVelocity();
        rigidbody.velocity = velocityEstimator.GetVelocityEstimate();
        rigidbody.useGravity = true;
        //(grabed=false)
    }
}
