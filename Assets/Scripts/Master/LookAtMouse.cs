using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {

    public Camera raycastOrigin;
    public float rotationSpeed = 1.0f;

    private Vector3 lastHitPoint = Vector3.zero;
    private Vector3 currVelocity = Vector3.zero;

	void Start () {
		
	}
	
	void Update () {
        if (!GameData.Instance.isPaused)
        {
            if (!raycastOrigin.enabled)
                return;

            // get aimed-for point via Raycast
            RaycastHit hit;
            Ray ray = raycastOrigin.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                lastHitPoint = hit.point;
            }

            if (Time.deltaTime > 0)
            {
                // rotate eye to point to raycast hit
                Quaternion targetRotation = Quaternion.LookRotation(lastHitPoint - raycastOrigin.transform.position);
                Vector3 targetEuler = targetRotation.eulerAngles;
                Vector3 euler = transform.eulerAngles;
                // what we would really want is Quaternion.SmoothDamp, but that does not exist
                // Vector3.SmoothDamp also kinda works, but is unreliable because it is not made for rotations
                euler.x = Mathf.SmoothDampAngle(euler.x, targetEuler.x, ref currVelocity.x, 0.15f, rotationSpeed, Time.deltaTime);
                euler.y = Mathf.SmoothDampAngle(euler.y, targetEuler.y, ref currVelocity.y, 0.15f, rotationSpeed, Time.deltaTime);
                euler.z = Mathf.SmoothDampAngle(euler.z, targetEuler.z, ref currVelocity.z, 0.15f, rotationSpeed, Time.deltaTime);
                transform.eulerAngles = euler;
            }
        }
    }
}
