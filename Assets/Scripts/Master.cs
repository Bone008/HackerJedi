using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour {

    public Camera masterCamera;
    public Transform masterEye;

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
        if(objectHit)
        {
            // TODO interpolate
            masterEye.LookAt(hit.point);
        }
    }
    
}
