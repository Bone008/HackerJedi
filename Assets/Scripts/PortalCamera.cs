using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// my own shitty attempts at creating a portal effect
// the render-only part could even work in theory, maybe
// but the Gater Plugin looks more promising
public class PortalCamera : MonoBehaviour {

    public Transform portalSurface;
    public Transform portalExit;
    public Transform sourceCamera;

	void Start () {
	}
	
	void LateUpdate () {
        //transform.rotation = portalExit.rotation;
        //transform.position = portalExit.position + (this.portalSurface.position - this.sourceCamera.position);

        //Find the position of the camera
        Vector3 pos = portalSurface.InverseTransformPoint(sourceCamera.position);
        transform.localPosition = new Vector3(-pos.x, pos.y, -pos.z);

        //Find the rotation
        Vector3 euler = Vector3.zero;
        euler.y = SignedAngle(-portalSurface.forward, sourceCamera.forward, Vector3.up);
        //TODO: Find the z-rotation

        transform.localRotation = Quaternion.Euler(euler);

    }

    private float SignedAngle(Vector3 a, Vector3 b, Vector3 n)
    {
        //Code stolen from DiegoSLTS
        //http://answers.unity3d.com/questions/992289/portal-effect-using-render-textures-how-should-i-m.html

        a.y = 0;
        b.y = 0;

        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        while (signed_angle < 0) signed_angle += 360;

        return signed_angle;
    }
}
