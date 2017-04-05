using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Scr : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        force();
	}
    public void force()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider col in cols)
        {
            if (col.tag == "Untagged")
            {
                col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(100, col.gameObject.transform.position, 5);
            }
        }
    }
}
