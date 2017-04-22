using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthResource))]
public class FirewallPart : MonoBehaviour {

    private HealthResource health;

	void Start () {
        health = GetComponent<HealthResource>();
        health.onDeath.AddListener(OnDeath);
    }
    
    public void OnDeath()
    {
        if (transform.parent.parent.childCount == 1)
            Destroy(transform.parent.parent.parent.gameObject); // sry
        else
            Destroy(transform.parent.gameObject);
    }
    
}
