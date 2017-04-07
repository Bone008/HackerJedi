using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillParticleSys : MonoBehaviour
{

    private ParticleSystem parSys;

	// Use this for initialization
	void Start ()
	{
	    parSys = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!parSys.IsAlive())
            Destroy(gameObject);
	}

}
