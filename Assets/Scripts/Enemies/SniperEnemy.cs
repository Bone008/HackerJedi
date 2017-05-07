using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class SniperEnemy : EnemyBase {

    public GameObject explo;
    public Gun weapon;

    public VolumetricLineBehavior aimLaser;
    public VolumetricLineBehavior preFireLaser;

    public GameObject nozzle;
    public GameObject eye;

    public float preAimTime = 2.0f;

    private Transform player;
    private CapsuleCollider playerCollider;
    private Platform platform;
    
    private enum SniperMode
    {
        DirectlyAimAtPlayer,
        Fire
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

        // start sniper coroutine
        StartCoroutine(DoSniperStuff());

        // set weapon to hit player
        weapon.layer = LayerMask.NameToLayer("Hacker");
    }
	
	void Update ()
    {
        // startposition of aimlaser is always the weapon
        aimLaser.StartPos = aimLaser.transform.InverseTransformPoint(weapon.transform.position);

        if (currentMode == SniperMode.DirectlyAimAtPlayer)
        {
            // rotate body towards player around y axis
            Vector3 bodyToPlayer = player.position - transform.position;
            bodyToPlayer.y = 0;
            Quaternion bodyRotation = Quaternion.LookRotation(bodyToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, bodyRotation, 2);

            // laser the shit out of player
            aimLaser.EndPos = aimLaser.transform.InverseTransformPoint(player.position - new Vector3(0, playerCollider.height / 3, 0));            
        }
        else if(currentMode == SniperMode.Fire)
        {
            // update start position of prefire laser
            if (preFireLaser.isActiveAndEnabled)
                preFireLaser.StartPos = preFireLaser.transform.InverseTransformPoint(eye.transform.position);
        }
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
            currentMode = SniperMode.DirectlyAimAtPlayer;
            // stuff in update()
            yield return new WaitForSeconds(3.0f);
            currentMode = SniperMode.Fire;

            // set preAimLaser position
            float dist = Vector3.Distance(nozzle.transform.position, player.transform.position);
            float totalTime = (dist / weapon.projectileSpeed) + preAimTime;
            Vector3 preAimPos = player.transform.position - new Vector3(0, playerCollider.height / 3, 0) + totalTime * platform.getVelocity();
            preFireLaser.gameObject.SetActive(true);
            preFireLaser.EndPos = preFireLaser.transform.InverseTransformPoint(preAimPos);

            // disable auto-rotation of weapon
            PointAtPlayer pap = weapon.gameObject.GetComponent<PointAtPlayer>();
            pap.enabled = false;

            // smooth animate weapon rotation and laser between current pos and hit pos
            Quaternion initialRotation = weapon.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(preAimPos - weapon.transform.position);
            Vector3 initialPos = aimLaser.EndPos;
            Vector3 targetPos = preAimPos;
            this.Animate(preAimTime, progress =>
            {
                weapon.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, progress);
                aimLaser.EndPos = Vector3.Lerp(initialPos, aimLaser.transform.InverseTransformPoint(targetPos), progress);
            });

            // fire after time has passed, disable laser, enable auto-rotation again
            this.Delayed(preAimTime, () =>
            {
                weapon.FireOnce();                             
            });
            this.Delayed(totalTime, () => {
                preFireLaser.gameObject.SetActive(false);
                pap.enabled = true;
            });

            yield return new WaitForSeconds(totalTime);
        }
    }
    
}
