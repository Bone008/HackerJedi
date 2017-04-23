using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuicideEnemy : EnemyBase {
    //public float newTargetPosThreshhold = 1;

    
    public float blinkRange;
    public float hitRange;
    private Transform platform;
    public Color color1;
    public Color color2;

    private NavMeshAgent agent;

    private Coroutine blinkCoroutine;

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

    void Start()
    {
        platform = GameObject.FindGameObjectWithTag("Platform").transform;
        //agent = GetComponent<NavMeshAgent>();

        //if (agent.isOnNavMesh)
        //    agent.destination = platform.position;

        GetComponent<Renderer>().material.color = color1;
    }

    void Update()
    {
        switch (suicideProgress)
        {
            case SuicideProgress.OnFloor:
                // set position above the enemy as target
                targetPosition = new Vector3(transform.position.x, targetHeight, transform.position.z);
                suicideProgress = SuicideProgress.GainingHeight;

                break;

            case SuicideProgress.GainingHeight:
                // move towards target
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed);

                // next move to platform
                if (transform.position == targetPosition)
                    suicideProgress = SuicideProgress.MoveAbovePlatform;

                break;

            case SuicideProgress.MoveAbovePlatform:
                // move towards platform
                targetPosition = new Vector3(platform.position.x, targetHeight, platform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed);

                // next fuck shit up
                if ((transform.position - targetPosition).sqrMagnitude <= startKamikazeDist * startKamikazeDist)
                    suicideProgress = SuicideProgress.Kamikaze;

                break;

            case SuicideProgress.Kamikaze:
                // start blinking
                if (blinkCoroutine == null)
                    blinkCoroutine = StartCoroutine(StartBlinking());

                // fly towards player
                targetPosition = platform.position;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed);

                // no next
                float sqDist = (platform.position - transform.position).sqrMagnitude;
                if (sqDist <= hitRange * hitRange)
                {
                    StopCoroutine(blinkCoroutine);
                    platform.GetComponent<Platform>().DisableForSec(2.0f);
                    Destroy(gameObject);
                    return;
                }

                break;
            
        }
        
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    private IEnumerator StartBlinking()
    {
        Renderer r = GetComponent<Renderer>();
        while (true)
        {
            r.material.color = color2;
            yield return new WaitForSeconds(0.4f);
            r.material.color = color1;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
