using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuicideEnemy : EnemyBase {

    public GameObject explo;
    public GameObject kamikazeExplo;
    public float hitRange;
    private Transform platform;
    public Color color1;
    public Color color2;

    private new Rigidbody rigidbody;

    private enum SuicideProgress
    {
        OnFloor,
        GainingHeight,
        MoveAbovePlatform,
        Kamikaze
    }
    private SuicideProgress suicideProgress = SuicideProgress.OnFloor;
    private Vector3 targetPosition;
    public float targetHeight;
    public float movementSpeed;
    public float startKamikazeDist;
    public float disablePlatformDuration;
    public AudioClip exploSound;

    private Throwable_OBJ throwable;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        platform = GameObject.FindGameObjectWithTag("Platform").transform;
       
        throwable = GetComponent<Throwable_OBJ>();
    }

    private void MoveTowards(Vector3 target)
    {
        var newPos = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        rigidbody.MovePosition(newPos);

        var dir = target - transform.position;
        dir.y = 0; // always look horizontally
        if(dir != Vector3.zero)
            rigidbody.MoveRotation(Quaternion.LookRotation(dir));
    }

    void Update()
    {
        if (throwable.IsGrabbed())
        {
            suicideProgress = SuicideProgress.OnFloor;
            return;
        }

        switch (suicideProgress)
        {
            case SuicideProgress.OnFloor:
                // set position above the enemy as target
                targetPosition = new Vector3(transform.position.x, targetHeight, transform.position.z);
                suicideProgress = SuicideProgress.GainingHeight;

                break;

            case SuicideProgress.GainingHeight:
                // move towards target
                MoveTowards(targetPosition);

                // next move to platform
                if (transform.position == targetPosition)
                    suicideProgress = SuicideProgress.MoveAbovePlatform;

                break;

            case SuicideProgress.MoveAbovePlatform:
                // move towards platform
                targetPosition = new Vector3(platform.position.x, targetHeight, platform.position.z);
                MoveTowards(targetPosition);

                // next fuck shit up
                if ((transform.position - targetPosition).sqrMagnitude <= startKamikazeDist * startKamikazeDist)
                    suicideProgress = SuicideProgress.Kamikaze;

                break;

            case SuicideProgress.Kamikaze:
                // fly towards player
                targetPosition = platform.position;
                MoveTowards(targetPosition);

                // no next
                float sqDist = (platform.position - transform.position).sqrMagnitude;
                if (sqDist <= hitRange * hitRange)
                {
                    var go = new GameObject("Explo Sound");
                    go.transform.position = this.transform.position;
                    var audio = go.AddComponent<AudioSource>();
                    audio.clip = exploSound;
                    audio.pitch = Random.Range(0.8f, 1.2f);
                    audio.Play();
                    this.Delayed(exploSound.length + 0.2f, () => Destroy(go));
                    platform.GetComponent<Platform>().DisableForSec(disablePlatformDuration);
                    Instantiate(kamikazeExplo, transform.position, transform.rotation);
                    Destroy(gameObject);
                    return;
                }

                break;
            
        }
        
    }

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
}
