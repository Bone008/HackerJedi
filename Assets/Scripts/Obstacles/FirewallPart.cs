using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class FirewallPart : MonoBehaviour {

    private Damageable damageable;

	void Start () {
        damageable = GetComponent<Damageable>();
        damageable.onDeath.AddListener(OnDeath);
    }
    
    public void OnDeath()
    {
        if (transform.parent.parent.childCount == 1)
            Destroy(transform.parent.parent.parent.gameObject); // sry
        else
            Destroy(transform.parent.gameObject);
    }
    
}
