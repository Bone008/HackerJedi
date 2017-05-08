using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderCamScript : MonoBehaviour {

    public bool on;


    public Transform startPoint, endPoint, lookAt, followObject;
    public Camera cam;
    public float moveSpeed = 3;
    public float rotateSpeed;
    public float stoppingDistance, fovChangeRate;
    public Vector3 followOffset;
    public int[] fovValues = { 48, 40 };
    public float camDepth = 1;

    public bool controls, fovChange;

    private bool manRotate;

	// Use this for initialization
	void Start () {

        cam.depth = camDepth;

        cam.enabled = on;

        if (lookAt != null)
            manRotate = false;
        else
            manRotate = true;        
        
        if (startPoint != null)
            transform.position = startPoint.position;
        else
            startPoint = transform;
	}
	
	// Update is called once per frame
	void Update () {

        float step = moveSpeed * Time.deltaTime;
        float fovStep = fovChangeRate * Time.deltaTime;

        if (controls)
            ControlManually(step);
        else if (endPoint != null && Vector3.Distance(transform.position, endPoint.position) > stoppingDistance)
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
        else if (followObject != null)
            transform.position = followObject.position + followOffset;

        if (fovChange)
            cam.fieldOfView += fovStep;

        if (lookAt != null && !manRotate)
            transform.LookAt(lookAt);

        if (Input.GetKeyDown("j"))
            cam.enabled = !cam.enabled;

        if (Input.GetKeyDown("m"))
            controls = !controls;

        if (Input.GetKeyDown("k"))
            manRotate = !manRotate;

        if (Input.GetKeyDown("n") && startPoint != null)
        {
            transform.position = startPoint.position;
            cam.fieldOfView = fovValues[1];
        }
            

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

        if (Input.GetKey(KeyCode.Keypad2) && manRotate)
            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.Keypad3) && manRotate)
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

    }
}
