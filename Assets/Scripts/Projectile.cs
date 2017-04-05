using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Projectile : MonoBehaviour {
    
    private float age = 0;

	void Start () {
	}

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collided with " + collider.gameObject.name);

        // for now: kill enemies instantly
        if (collider.gameObject.GetComponent<Enemy>() != null)
            Destroy(collider.gameObject);

        Destroy(this.gameObject);
    }


    void Update () {
        age += Time.deltaTime;
        if (age > 5)
        {
            Destroy(this.gameObject);
            return;
        }
	}
}
