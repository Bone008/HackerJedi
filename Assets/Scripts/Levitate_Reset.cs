using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levitate_Reset : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void init(Throwable_OBJ[] levitated)
    {
        StartCoroutine(Reset(levitated));
    }
    
    IEnumerator Reset(Throwable_OBJ[] levitated)
    {
        yield return new WaitForSeconds(9);
        foreach (Throwable_OBJ col in levitated)
        {
            if(col != null)
                col.setFree();
        }
    }
}
