using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Update is called once per frame
	void Awake () {
        StartCoroutine(Levitate());
	}
    IEnumerator Levitate()
    {
        yield return new WaitForSeconds(1);
    }
}
