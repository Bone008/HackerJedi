using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragmentation_Saw : MonoBehaviour {
    public float upperPoint;
    public float lowerPoint;
    private bool up=false;
    public float speed = 0.05f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y > upperPoint)
            up = false;
        if (transform.position.y < lowerPoint)
            up = true;

        if (up)
        {
            transform.Translate(0, 0, speed);//Vector3.Slerp(transform.position, new Vector3(transform.position.x, upperPoint, transform.position.z), Time.deltaTime));
        }
        else
        {
            transform.Translate(0, 0, -speed);//Vector3.Slerp(transform.position, new Vector3(transform.position.x, lowerPoint, transform.position.z), Time.deltaTime));
        }
        transform.GetChild(1).Rotate(-5, 0, 0);
	}
}
