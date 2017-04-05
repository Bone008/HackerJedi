using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {

    public Camera raycastOrigin;
    public float rotationSpeed = 1.0f;

	void Start () {
		
	}
	
	void Update () {
        // get aimed-for object via Raycast
        Transform objectHit = null;
        RaycastHit hit;
        Ray ray = raycastOrigin.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            objectHit = hit.transform;
        }

        // rotate eye to point to raycast hit
        if (objectHit)
        {
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - raycastOrigin.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
