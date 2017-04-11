using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class Regenerative : MonoBehaviour {

    [Tooltip("Wait this amount of secs after being damaged before restoring health")]
    public float waitAfterDamageS;

    [Tooltip("Wait this amount of secs after regeneration before regenerating again")]
    public float intervalS;

    [Tooltip("When regenerating, this amount of health is restored")]
    public float healthAmountPerInterval;
    
    private Damageable damageable;
    private float currentWaitAfterDamageS;
    private float currentIntervalS;
    
	void Start () {
        // get damageable for health stuff
        damageable = GetComponent<Damageable>();
        damageable.onDamage.AddListener(OnDamage);

        // instant regeneration
        currentWaitAfterDamageS = 0;

        // wait at least one interval
        currentIntervalS = intervalS;
	}
	
	void Update () {
        bool regenerate = true;

        // check if too early after damage
        currentWaitAfterDamageS -= Time.deltaTime;
        if (currentWaitAfterDamageS > 0)
            regenerate = false;

        // check if too early after last regeneration
        currentIntervalS -= Time.deltaTime;
        if (currentIntervalS > 0)
            regenerate = false;

        // check if full health
        if (damageable.currentHealth >= damageable.initialHealth)
            regenerate = false;

        if (regenerate)
        {
            damageable.ChangeHealth(healthAmountPerInterval);
            currentIntervalS = intervalS;
        }
    }

    void OnDamage()
    {
        currentWaitAfterDamageS = waitAfterDamageS;
    }
}
