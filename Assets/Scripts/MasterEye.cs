﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEye : MonoBehaviour {

    public float maxHealth = 1000.0f;
    public GameObject deathExplosion;

    private float currentHealth;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
	}

    public void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Debug.Log("master deadeded");
            // TODO
            currentHealth = maxHealth;
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
        }

        // TODO remove
        Debug.Log("Master hurt, has now " + currentHealth + " health");
    }
    
}
