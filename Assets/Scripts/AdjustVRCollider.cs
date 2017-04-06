using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustVRCollider : MonoBehaviour {

    public Transform head;
    public float overHeadOffset = 0.2f;

    private CapsuleCollider capsCollider;

	void Start () {
        capsCollider = GetComponent<CapsuleCollider>();
	}
	
	void Update () {
        // scale collider to stick on the ground and to stay vertical
        capsCollider.height = head.localPosition.y + overHeadOffset;
        capsCollider.center = new Vector3(0, - capsCollider.height / 2 + overHeadOffset, 0);
        capsCollider.transform.rotation = Quaternion.identity;
	}
}
