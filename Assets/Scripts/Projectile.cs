using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Projectile : MonoBehaviour {

    [HideInInspector]
    public float damageAmount = 1.0f;

    private float age = 0;

	void Start () {
	}

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("collided with " + collider.gameObject.name);

        // dont hit gun itself
        if (collider.GetComponentInParent<Gun>() != null)
            return;

        if (collider.GetComponent<ShieldPanel>() != null)
            return;

        HealthResource health = collider.gameObject.GetComponentInParent<HealthResource>();
        if (health != null)
            health.ChangeValue(-damageAmount);

        /*
        // damage enemies
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy != null)
            enemy.OnDamage(damageAmount);
        SuicideEnemy enemy2 = collider.gameObject.GetComponent<SuicideEnemy>();
        if (enemy2 != null)
            enemy2.OnDamage(damageAmount);

        // damage player
        HackerPlayer player = collider.gameObject.GetComponent<HackerPlayer>();
        if (player != null)
            player.OnDamage(damageAmount);

        // damage master
        MasterEye masterEye = collider.gameObject.GetComponentInParent<MasterEye>();
        if (masterEye != null)
            masterEye.OnDamage(damageAmount);
        */

        Destroy(gameObject);
    }


    void Update () {
        age += Time.deltaTime;
        if (age > 5)
        {
            Destroy(gameObject);
            return;
        }
	}
}
