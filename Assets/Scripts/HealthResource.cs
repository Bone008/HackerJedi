﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthResource : ResourceBase {
    
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public AudioClip[] playerHit;

    void Start () {
        RestoreFullHealth();
	}
    
    public void RestoreFullHealth()
    {
        ChangeValue(maxValue - currentValue);
    }

    /*public void playAudio()
    {
        var rnd = UnityEngine.Random.Range(0, 2);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        AudioSource.PlayClipAtPoint(playerHit[rnd], player.transform.position, 1f);
    }*/

    public override bool ChangeValue(float amount)
    {
        bool hasChanged = base.ChangeValue(amount);

        // notify additional listeners
        if (hasChanged)
        {
            if (onDamage != null && amount < 0)
                onDamage.Invoke();

            if (onDeath != null && currentValue <= 0)
                onDeath.Invoke();
        }

        return hasChanged;
    }

    public override bool SafeChangeValue(float amount)
    {
        throw new InvalidOperationException("SafeChangeValue should not be needed™ for health!");
    }

}
