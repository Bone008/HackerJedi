using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillParticleSys : MonoBehaviour
{

    private float startTime;
    private ParticleSystem parSys;

	// Use this for initialization
	void Start ()
	{
	    startTime = Time.time;
	    parSys = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!parSys.IsAlive())
            Destroy(gameObject);
	}

}
