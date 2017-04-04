using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public Transform startPoint;
    public Transform endPoint;
    public float velocity;
    
    void Start () {
        transform.position = startPoint.position;
	}
	
	void Update () {
        float duration = (endPoint.position - startPoint.position).magnitude / velocity;
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, Mathf.PingPong(Time.timeSinceLevelLoad / duration, 1.0f));
	}
}
