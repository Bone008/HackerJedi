using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCollider : MonoBehaviour {

    public float damageAmount = 200;

    private void OnTriggerEnter(Collider other)
    {
        HackerPlayer player = other.gameObject.GetComponent<HackerPlayer>();

        if (player != null)
        {
            player.GetComponent<Damageable>().ChangeHealth(-damageAmount);

            if (transform.parent.parent.childCount == 1)
                Destroy(transform.parent.parent.parent.gameObject); // sry
            else
                Destroy(transform.parent.gameObject);
        }
    }
}
