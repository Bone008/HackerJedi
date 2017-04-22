using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathFeedDataFragments : MonoBehaviour {

    public float amount;

    void Start ()
    {
        // set ondeath listener
        Damageable damageable = GetComponent<Damageable>();
        damageable.onDeath.AddListener(FeedDataFragments);
	}
    
    private void FeedDataFragments()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        DataFragmentResource resource = playerGO.GetComponent<DataFragmentResource>();
        resource.ChangeValue(amount);
    }

}
