using System.Collections;
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
	    parent = transform.parent.gameObject;
	    platform = GameObject.Find("Platform");
        //platformVelocity = platform.GetComponent<Platform>().getVelocity();
	}
	
	// Update is called once per frame
	void Update ()
	{

	    float dist = Vector3.Distance(transform.position, player.transform.position);
	    float timeOffset = dist/bulletVelocity;
	    Vector3 aimPosition = player.transform.position + timeOffset * platform.GetComponent<Platform>().getVelocity();

        Debug.Log("Player pos " + player.transform.position + " timeDelta " + timeOffset + " platV " + platform.GetComponent<Platform>().getVelocity());
	    Debug.Log("predicted pos " + aimPosition);
        
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
