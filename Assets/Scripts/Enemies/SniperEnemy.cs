using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class SniperEnemy : EnemyBase {

    public GameObject explo;
    public Gun weapon;
    public VolumetricLineBehavior laser;
    public float cooldown;

    private Transform player;

	void Start ()
    {
        // set player as target
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        player = playerGO.transform;
    }
	
	void Update ()
    {
        // rotate body towards player around y axis
        Vector3 bodyToPlayer = player.position - transform.position;
        bodyToPlayer.y = 0;
        Quaternion bodyRotation = Quaternion.LookRotation(bodyToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, bodyRotation, 0.2f);
        
        // render laser
        RaycastHit hit;
        if (Physics.Raycast(weapon.transform.position, weapon.transform.forward, out hit, 500))
        {
            laser.SetStartAndEndPoints(laser.transform.InverseTransformPoint(weapon.transform.position), laser.transform.InverseTransformPoint(hit.point));            
        }

        // fire when ready
        weapon.FireOnce();
    }

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        Destroy(transform.parent.gameObject);
    }
}
