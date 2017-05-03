using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCollider : MonoBehaviour {

    public float damageAmount = 200;

    private void OnTriggerEnter(Collider other)
    {
        HackerPlayer player = other.gameObject.GetComponentInParent<HackerPlayer>();

        if (player != null)
        {
            player.GetComponent<HealthResource>().ChangeValue(-damageAmount);

            if (transform.parent.childCount == 1)
                Destroy(transform.parent.parent.gameObject);
            else
                Destroy(gameObject);
        }
    }
}
