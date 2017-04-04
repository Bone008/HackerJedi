using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour {

    public Camera masterCamera;
    public Transform masterEye;

    public float rotationSpeed;

    [Header("Movement")]
    public Transform movementMin;
    public Transform movementMax;
    public float movementSpeed;

	void Start () {
		
	}
	
	void Update () {
        // get aimed-for object via Raycast
        Transform objectHit = null;
        RaycastHit hit;
        Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            objectHit = hit.transform;
        }

        // left mouse button down
        if (objectHit != null && Input.GetMouseButtonDown(0))
        {
            // TODO
            objectHit.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        }

        // rotate eye to point to raycast hit
        if (objectHit)
        {
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - masterEye.position);
            masterEye.rotation = Quaternion.Slerp(masterEye.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // move eye with A / D buttons
        // TODO nice to have: accelerate
        if (Input.GetKey(KeyCode.W) && transform.position.z < movementMax.position.z)
            transform.position +=new Vector3(0, 0, movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S) && transform.position.z > movementMin.position.z)
            transform.position -= new Vector3(0, 0, movementSpeed * Time.deltaTime);
        
    }
    
}
