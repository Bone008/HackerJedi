using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : MonoBehaviour {

    public GameObject explo;

    private Transform player;

	void Start ()
    {
        // set player as target
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        player = playerGO.transform;
    }
	
	void Update ()
    {
        // look at player
        transform.LookAt(player);
	}

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
