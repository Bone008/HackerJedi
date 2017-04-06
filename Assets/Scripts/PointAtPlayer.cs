﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtPlayer : MonoBehaviour
{
    
    public float aimingFOV = 45, bulletVelocity = 40;
    //private Vector3 platformVelocity;
    private GameObject parent, player, platform;

	// Use this for initialization
	void Start ()
	{
        player = GameObject.FindWithTag("Player");
        // if there is no player, there is no meaning to our life
        if (player == null)
        {
            enabled = false;
            return;
        }

        parent = transform.parent.gameObject;
	    platform = GameObject.Find("Platform");
        //platformVelocity = platform.GetComponent<Platform>().getVelocity();
	}
	
	// Update is called once per frame
	void Update ()
	{

	    float dist = Vector3.Distance(transform.position, player.transform.position);
	    float timeOffset = dist/bulletVelocity;

        var vrCollider = player.GetComponent<AdjustVRCollider>();
        float overHeadOffset = (vrCollider != null ? vrCollider.overHeadOffset : 0);
	    Vector3 aimPosition = player.transform.position - new Vector3(0, overHeadOffset, 0) + timeOffset * platform.GetComponent<Platform>().getVelocity();
        
	    if (Vector3.Angle(parent.transform.forward, (player.transform.position - parent.transform.position)) < aimingFOV)
	    {
	        transform.LookAt(aimPosition);
	    }
	    else
	    {
	        transform.forward = parent.transform.forward;
	    }
	}
}
