using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private float age = 0;

	void Start () {
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
