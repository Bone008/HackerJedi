using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : AbstractAbility {

    public AbilityType abilityType = AbilityType.SimpleGun;

    public AudioClip shootSound;
    public GameObject projectilePrefab;
    public Transform nozzle;
    public float projectileSpeed = 1.0f;
    public float damageAmount = 25.0f;

    public override AbilityType Type { get { return abilityType; } }

    [HideInInspector]
    public int? layer = null;

    private void Start()
    {
        if(!layer.HasValue)
            layer = gameObject.layer;
    }

    protected override void OnTriggerDown()
    {
        var aimRay = GetAimRay(nozzle);
        var shootingDirection = aimRay.direction;
        var position = aimRay.origin;
        var rotation = Quaternion.LookRotation(shootingDirection);
        
        GameObject projectile = GameObject.Instantiate(projectilePrefab, position, rotation);
        projectile.GetComponent<Rigidbody>().velocity = shootingDirection * projectileSpeed;

        // store damage amount of gun in projectile
        projectile.GetComponent<Projectile>().damageAmount = damageAmount;

        // store layer
        if(layer.HasValue)
            projectile.layer = layer.Value;

        if (shootSound != null)
        {
            //AudioSource.PlayClipAtPoint(shootSound, nozzle.position, 0.5f);
            var go = new GameObject("Shoot sound");
            go.transform.position = nozzle.position;
            var audio = go.AddComponent<AudioSource>();
            audio.clip = shootSound;
            audio.volume = 0.3f;
            audio.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            audio.Play();
            this.Delayed(shootSound.length + 0.5f, () => Destroy(go));
        }
    }

}
