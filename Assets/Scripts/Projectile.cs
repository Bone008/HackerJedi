using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    
    private float age = 0;

	void Start () {
	}

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collided with " + collider.gameObject.name);

        // TODO damage enemies once we have them

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
