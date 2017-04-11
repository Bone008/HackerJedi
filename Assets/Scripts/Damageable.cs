using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour {

    public float initialHealth;

    // TODO better way of implementing get/private set?
    private float _currentHealth;
    public float currentHealth
    {
        get { return _currentHealth; }
        private set { _currentHealth = value; }
    }

    public float healthPercentage
    {
        get { return Mathf.Max(0, currentHealth) / initialHealth; }
    }

    public UnityEvent onPostDamage;
    public UnityEvent onDeath;

    void Start () {
        RestoreFullHealth();
	}
	
    public void RestoreFullHealth()
    {
        currentHealth = initialHealth;
    }

    public void OnDamage(float damageAmount)
    {
        // damage
        currentHealth -= damageAmount;
        if (onPostDamage != null)
            onPostDamage.Invoke();

        // death
        if (currentHealth < 0)
        {
           if (onDeath != null)
                onDeath.Invoke();
            return;
        }
    }

}
