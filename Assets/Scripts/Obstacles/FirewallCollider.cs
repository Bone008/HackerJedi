using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCollider : MonoBehaviour {

    public float damageAmount = 200;
    private float noDamageTime=0;

    void Update()
    {
        noDamageTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (noDamageTime > 0)
        {
            return;
        }
        noDamageTime = 2;
        HackerPlayer player = other.gameObject.GetComponentInParent<HackerPlayer>();

        if (player != null)
        {
            player.GetComponent<HealthResource>().ChangeValue(-damageAmount);
            Debug.Log("Hacker was hit by Sawblade");
            //if (transform.parent.childCount == 1)
            //    Destroy(transform.parent.parent.gameObject);
            //else
            //    Destroy(gameObject);
        }
    }
}
