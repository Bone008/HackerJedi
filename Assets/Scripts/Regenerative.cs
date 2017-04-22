using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceBase))]
public class Regenerative : MonoBehaviour {
    
    [Tooltip("Wait this amount of secs after regeneration before regenerating again")]
    public float intervalS;

    [Tooltip("When regenerating, this amount is restored")]
    public float amountPerInterval;

    public ResourceBase resourceToRegenerate;
    private float currentWaitAfterDamageS;
    private float currentIntervalS;
    
	void Start () {
        // get damageable for health stuff
        resourceToRegenerate = GetComponent<ResourceBase>();

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
        if (resourceToRegenerate.currentValue >= resourceToRegenerate.maxValue)
            regenerate = false;

        if (regenerate)
        {
            resourceToRegenerate.ChangeValue(amountPerInterval);
            currentIntervalS = intervalS;
        }
    }

    public void BlockForSecs(float blockTimeSec)
    {
        currentWaitAfterDamageS = blockTimeSec;
    }
}
