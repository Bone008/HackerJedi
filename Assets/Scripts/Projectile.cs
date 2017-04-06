﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Projectile : MonoBehaviour {

    [HideInInspector]
    public float damageAmount = 1.0f;

    private float age = 0;

	void Start () {
	}

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collided with " + collider.gameObject.name);

        // damage enemies
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy != null)
            enemy.OnDamage(damageAmount);

        // damage player
        HackerPlayer player = collider.gameObject.GetComponent<HackerPlayer>();
        if (player != null)
            player.OnDamage(damageAmount);

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
