using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour {

    // TODO better way of implementing get/private set/public in inspector?
    [SerializeField]
    private float _initialHealth;
    public float initialHealth
    {
        get { return _initialHealth; }
        private set { _initialHealth = value; }
    }

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

    public UnityEvent onHealthChanged;
    public UnityEvent onDamage;
    public UnityEvent onDeath;

    void Start () {
        RestoreFullHealth();
	}
    
    public void RestoreFullHealth()
    {
        currentHealth = initialHealth;

        if (onHealthChanged != null)
            onHealthChanged.Invoke();
    }

    public void ChangeHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, initialHealth);
        
        if (onHealthChanged != null)
            onHealthChanged.Invoke();

        if (onDamage != null && amount < 0)
            onDamage.Invoke();

        if (onDeath != null && currentHealth <= 0)
            onDeath.Invoke();
    }

}
