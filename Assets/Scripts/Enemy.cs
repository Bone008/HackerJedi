using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Transform target;
    public float hitRange;
    public float movementSpeed;

    private Gun gun;

	void Start () {
        // get gun component from children
        gun = GetComponentInChildren<Gun>();
	}
	
	void Update () {
        // move towards and look to target
        transform.position = Vector3.MoveTowards(transform.position, target.position, movementSpeed);
        transform.LookAt(target);

        // fire if in range
		if((target.position - transform.position).magnitude <= hitRange)
        {
            gun.Fire();
        }
	}
}
