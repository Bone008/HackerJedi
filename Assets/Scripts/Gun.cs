using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public GameObject projectilePrefab;
    public Transform nozzle;
    public float projectileSpeed = 1.0f;
    public float damageAmount = 25.0f;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            Fire();
	}

    public void Fire()
    {
        var shootingDirection = nozzle.transform.TransformDirection(Vector3.forward).normalized;
        var position = nozzle.position + shootingDirection * 1.1f * projectilePrefab.transform.localScale.y;
        var rotation = Quaternion.LookRotation(shootingDirection) * Quaternion.Euler(90, 0, 0);
        
        GameObject projectile = GameObject.Instantiate(projectilePrefab, position, rotation);
        projectile.GetComponent<Rigidbody>().velocity = shootingDirection * projectileSpeed;

        // store damage amount of gun in projectile
        projectile.GetComponent<Projectile>().damageAmount = damageAmount;
    }
}
