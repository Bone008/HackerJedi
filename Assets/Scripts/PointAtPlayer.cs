using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointAtPlayer : MonoBehaviour
{
    
    public float bulletVelocity;
    private float platformSpeed;
    private GameObject parent, player, platform;

	// Use this for initialization
	void Start ()
	{
        player = GameObject.FindWithTag("Player");
	    parent = transform.parent.gameObject;
	    platform = GameObject.Find("Platform");
        platformSpeed = platform.GetComponent<Platform>().velocity;
	}
	
	// Update is called once per frame
	void Update ()
	{

	    float dist = Vector3.Distance(transform.position, player.transform.position);
	    float offset = dist/bulletVelocity;
	    Vector3 aimPosition = player.transform.position + offset * (new Vector3(0,0,1*platformSpeed));  //platform move vector.
        
        //Debug
	    Debug.Log("angle to player" + Vector3.Angle(parent.transform.forward, (player.transform.position - parent.transform.position)));
	    Debug.Log("Vec to player" + (player.transform.position - parent.transform.position));

	    if (Vector3.Angle(parent.transform.forward, (player.transform.position - parent.transform.position)) < 45)
	    {
	        transform.LookAt(aimPosition);
	    }
	    else
	    {
	        transform.forward = parent.transform.forward;
	    }
        
	    Debug.Log("player pos: " + player.transform.position);
        Debug.Log("aiming pos: " + aimPosition);
	}
}
