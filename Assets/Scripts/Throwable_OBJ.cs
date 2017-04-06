using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;
using UnityEngine.AI;

//for throwing the enemy
[RequireComponent(typeof(VelocityEstimator))]
[RequireComponent(typeof(Rigidbody))]
//end
public class Throwable_OBJ : MonoBehaviour
{

    private const float recoveryTotalTime = 2;

    private VelocityEstimator velocityEstimator; //for throwing the enemy
    private new Rigidbody rigidbody;
    private NavMeshAgent navAgent;

    private float recoveryStartTime = 0;
    private bool isRecoveringFromThrow = false;

    void Awake()
    {
        velocityEstimator = GetComponent<VelocityEstimator>();
        rigidbody = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        if(isRecoveringFromThrow && Time.time - recoveryStartTime >= recoveryTotalTime && rigidbody.velocity.sqrMagnitude < 0.2f * 0.2f)
        {
            isRecoveringFromThrow = false;
            if (navAgent != null)
                navAgent.enabled = true;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Throwing-Functionality
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void setGrabbed()
    {
        if (isRecoveringFromThrow)
            isRecoveringFromThrow = false;

        velocityEstimator.BeginEstimatingVelocity();
        rigidbody.useGravity = false;
        //rigidbody.isKinematic = true;

        if (navAgent != null)
            navAgent.enabled = false;
    }

    public void setFree()
    {
        velocityEstimator.FinishEstimatingVelocity();
        rigidbody.velocity = velocityEstimator.GetVelocityEstimate();
        rigidbody.useGravity = true;
        //rigidbody.isKinematic = false;

        recoveryStartTime = Time.time;
        isRecoveringFromThrow = true;
    }
}
