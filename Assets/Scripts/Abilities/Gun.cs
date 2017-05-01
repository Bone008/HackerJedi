using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : AbstractAbility {

    public AudioClip shootSound;
    public GameObject projectilePrefab;
    public Transform nozzle;
    public float magazineEmptyCooldown;
    public int magazineSize;
    public float projectileSpeed;
    public float damageAmount;

    [HideInInspector]
    public int? layer = null;

    private float lastShotFired = 0.0f;
    private int shotsFired = 0; // how many rounds have been fired in the current magazine?

    private void Start()
    {
        if(!layer.HasValue)
            layer = gameObject.layer;
    }

    private void Update()
    {
        // if the "magazine" has been partially emptied and no shots have been fired for one entire cooldown duration, reset magazine
        if(shotsFired > 0 && Time.timeSinceLevelLoad - lastShotFired >= magazineEmptyCooldown)
        {
            shotsFired = 0;
        }
    }

    protected override void OnTriggerDown()
    {
        if (IsCoolingDown)
            return;

        lastShotFired = Time.timeSinceLevelLoad;
        shotsFired++;
        if(shotsFired >= magazineSize)
        {
            CooldownFor(magazineEmptyCooldown);
            shotsFired = 0;
        }

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
