using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderCamScript : MonoBehaviour {

    public Transform startPoint, endPoint, lookAt, followObject;
    public float moveSpeed = 3;
    public float stoppingDistance;
    public Vector3 followOffset;

    public bool controls;

	// Use this for initialization
	void Start () {

        if (startPoint != null)
            transform.position = startPoint.position;
        else
            startPoint = transform;
	}
	
	// Update is called once per frame
	void Update () {

        float step = moveSpeed * Time.deltaTime;

        if (controls)
            ControlManually(step);
        else if (endPoint != null && Vector3.Distance(transform.position, endPoint.position) > stoppingDistance)
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
        else if (followObject != null)
            transform.position = followObject.position + followOffset;

        if (lookAt != null)
            transform.LookAt(lookAt);

        if (Input.GetKeyDown("m"))
            controls = !controls;

        if (Input.GetKeyDown("n"))
            transform.position = startPoint.position;

	}


    private void ControlManually(float step)
    {
        if (Input.GetKey("up"))
            transform.position += transform.forward * step;

        if (Input.GetKey("down"))
            transform.position -= transform.forward * step;

        if (Input.GetKey("left"))
            transform.position -= transform.right * step;

        if (Input.GetKey("right"))
            transform.position += transform.right * step;

        if (Input.GetKey(KeyCode.Keypad0))
            transform.position -= transform.up * step;

        if (Input.GetKey(KeyCode.Keypad1))
            transform.position += transform.up * step;

    }
}
