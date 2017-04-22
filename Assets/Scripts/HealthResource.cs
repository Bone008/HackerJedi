﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthResource : ResourceBase {
    
    public UnityEvent onDamage;
    public UnityEvent onDeath;

    void Start () {
        RestoreFullHealth();
	}
    
    public void RestoreFullHealth()
    {
        ChangeValue(maxValue - currentValue);
    }

    public override void ChangeValue(float amount)
    {
        base.ChangeValue(amount);        

        if (onDamage != null && amount < 0)
            onDamage.Invoke();

        if (onDeath != null && currentValue <= 0)
            onDeath.Invoke();
    }

    public override bool SafeChangeValue(float amount)
    {
        throw new InvalidOperationException("SafeChangeValue should not be needed™ for health!");
    }

}