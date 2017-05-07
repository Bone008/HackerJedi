using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtPlayer : MonoBehaviour
{
    
    public float aimingFOV = 45, bulletVelocity = 40;

    /// <summary>Transform of the agent that this controller belongs to.</summary>
    public Transform selfContext;
    /// <summary>Axis in local space where "forward" should be.</summary>
    public Vector3 forwardDirection;
    
    private GameObject player;
    private Platform platform;

    private Quaternion initialLocalRotation;

	// Use this for initialization
	void Start ()
	{
        initialLocalRotation = transform.localRotation;

        player = GameObject.FindWithTag("Player");
        // if there is no player, there is no meaning to our life
        if (player == null)
        {
            enabled = false;
            return;
        }
        
	    platform = GameObject.FindWithTag("Platform").GetComponent<Platform>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float dist = Vector3.Distance(transform.position, player.transform.position);
	    float timeOffset = dist/bulletVelocity;

        var vrCollider = player.GetComponent<AdjustVRCollider>();
        float overHeadOffset = (vrCollider != null ? vrCollider.overHeadOffset : 0);
	    Vector3 aimPosition = player.transform.position - new Vector3(0, overHeadOffset, 0) + timeOffset * platform.GetComponent<Platform>().getVelocity();
        
	    if (Vector3.Angle(selfContext.forward, (player.transform.position - selfContext.position)) < aimingFOV)
	    {
            Debug.DrawLine(transform.position, aimPosition);
            transform.rotation = Quaternion.LookRotation(aimPosition - transform.position) * Quaternion.LookRotation(-forwardDirection) * Quaternion.Euler(0, 90, 0);
	    }
	    else
	    {
            transform.localRotation = initialLocalRotation;
        }
	}
}
