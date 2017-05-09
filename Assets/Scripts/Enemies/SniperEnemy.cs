using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class SniperEnemy : EnemyBase {

    public GameObject explo;
    public Gun weapon;

    public VolumetricLineBehavior aimLaser;

    public GameObject nozzle;
    public GameObject eye;

    public float preAimTime = 2.0f;
    public float preFireTime = 2.0f;
    public float cooldownTime = 2.0f;

    private Transform player;
    private CapsuleCollider playerCollider;
    private Platform platform;

    public Color colorPreaim;
    public Color colorShoot;
    
    private enum SniperMode
    {
        Preaim,
        PrepareToFire,
        Cooldown
    };
    private SniperMode currentMode;

	void Start ()
    {
        // set player as target
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        player = playerGO.transform;
        playerCollider = playerGO.GetComponent<CapsuleCollider>();

        // get platform
        platform = GameObject.FindGameObjectWithTag("Platform").GetComponent<Platform>();
        
        // set weapon to hit player
        weapon.layer = LayerMask.NameToLayer("Hacker");

        // set default color
        aimLaser.LineColor = colorPreaim;

        // start sniper coroutine
        StartCoroutine(DoSniperStuff());        
    }
	
	void Update ()
    {
        // startposition of aimlaser is always the weapon
        aimLaser.StartPos = aimLaser.transform.InverseTransformPoint(weapon.transform.position);

        if (currentMode == SniperMode.Preaim || currentMode == SniperMode.Cooldown)
        {
            // set preaim position
            float dist = Vector3.Distance(nozzle.transform.position, player.transform.position);
            float totalTime = (dist / weapon.projectileSpeed) + preFireTime;
            Vector3 preAimPos = player.transform.position - new Vector3(0, playerCollider.height / 3, 0) + totalTime * platform.getVelocity();
            aimLaser.EndPos = (aimLaser.transform.InverseTransformPoint(preAimPos) - aimLaser.StartPos) * 10;
        }
        
        // rotate body towards player around y axis
        Vector3 bodyToPlayer = player.position - transform.position;
        bodyToPlayer.y = 0;
        Quaternion bodyRotation = Quaternion.LookRotation(bodyToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, bodyRotation, 2);

        // rotate weapon to follow laser
        weapon.transform.rotation = Quaternion.LookRotation(aimLaser.transform.TransformPoint(aimLaser.EndPos) - weapon.transform.position);
    }

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        Destroy(transform.parent.gameObject);
    }

    private IEnumerator DoSniperStuff()
    {
        while (true)
        {
            // == Preaim ==
            currentMode = SniperMode.Preaim;
            aimLaser.LineColor = colorPreaim;
            yield return new WaitForEndOfFrame(); // wait for update of endpos of laser before enabling again
            aimLaser.gameObject.SetActive(true);
            yield return new WaitForSeconds(preAimTime);

            // == Prepare to Fire
            currentMode = SniperMode.PrepareToFire;
            aimLaser.LineColor = colorShoot;
            yield return this.Delayed(preFireTime, () =>
            {
                // == Fire ==
                currentMode = SniperMode.Cooldown;
                weapon.FireOnce();
                aimLaser.gameObject.SetActive(false);
            });

            yield return new WaitForSeconds(cooldownTime);
        }
    }
    
}
