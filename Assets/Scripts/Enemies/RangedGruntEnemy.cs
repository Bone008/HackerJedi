﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedGruntEnemy : EnemyBase
{
    public float newTargetPosThreshhold = 1f;
    public float rotationSpeed = 5f;
    public Vector3 offset = new Vector3(.5f, 1.533333f, .5f);
    public float hitRange, stoppingDistance, floorTime, recoveryMovementSpeed, damageMinSpeed, fallingDamageMultiplier;
    public Vector3 bounds;

    public GameObject explo;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private HealthResource health;
    private bool recovering, timerRunning;
    private float startTimeResting = 0;
    private int collisions = 0;
    private bool shootingEnabled = true;

    [Header("Upgrade")]
    public GameObject leftGun;
    public PointAtPlayer pap;

    void Start()
    {
        // locate player
        var player = GameObject.FindGameObjectWithTag("Player");
        // if there is no player, there is no meaning to our life
        if (player == null)
        {
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        health = GetComponent<HealthResource>();
       
        recovering = false;
        timerRunning = false;
        goal = player.transform;
        oldPos = goal.position;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;

        if (agent.isOnNavMesh)
            agent.destination = goal.position;

        // activated 2nd gun if upgrade
        if (GameData.Instance.betterRangedGruntUnlocked)
        {
            leftGun.SetActive(true);
            pap.enabled = true;
        }
    }

    void Update()
    {
        
        if (agent.enabled)
        {
            SetShootingEnabled(true);

            //Handle enemy in functional state and on navmesh
            float dist = Vector3.Distance(transform.position, goal.position);
            if (agent.isOnNavMesh)
            {
                handleFunctionalEnemy(dist);
            }
        }
        else
        {
            SetShootingEnabled(false);

            //Handle enemy in non-functional state and/or off navmesh
            handleDisabledEnemy();
        }        
    }
    
    private void rotateTowards(Transform target)
    {
        Vector3 targetDir = target.position - transform.position;
        targetDir = new Vector3(targetDir.x, 0, targetDir.z);
        float step = rotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);        
        transform.rotation = Quaternion.LookRotation(newDir);        
    }

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        //TODO: Wait for sound to play before object is destroyed
        //TODO instantiate sound object
        Destroy(gameObject);
        
    }

    public void SetShootingEnabled(bool enabled)
    {
        if (shootingEnabled != enabled)
        {
            foreach (var gun in GetComponentsInChildren<Gun>())
                gun.enabled = enabled;
            shootingEnabled = enabled;
        }
    }

    private void handleFunctionalEnemy(float dist)
    {
        //Stop enemy if close enough and has line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, goal.position - transform.position, out hit, 100.0f))
        {
            if (dist < stoppingDistance && !agent.isStopped && hit.collider.CompareTag("Player"))
            {
                agent.isStopped = true;
            }
        }
            

        

        //follow player if distance too big
        if (dist > hitRange && agent.isStopped)
        {
            agent.isStopped = false;
            agent.destination = goal.position;
            oldPos = goal.position;
        }

        //Only update goal position if platform moved reasonable amount
        if (Vector3.Distance(goal.position, oldPos) > newTargetPosThreshhold && !agent.isStopped)
        {
            agent.destination = goal.position;
            oldPos = goal.position;
        }        

        //Always look at player when in hit range
        if (dist < hitRange)
        {
            agent.updateRotation = false;
            rotateTowards(goal);
        }
        else
        {
            agent.updateRotation = true;
        }        
    }


    private void handleDisabledEnemy()
    {
        if (!recovering)
        {           
            if (rb.velocity.sqrMagnitude < .1f)
            {
                if (!timerRunning && collisions > 0)
                {
                    startTimeResting = Time.time;
                    timerRunning = true;
                }
                if (timerRunning && collisions == 0)
                {                
                    timerRunning = false;
                }

                if (collisions > 0 && Time.time - startTimeResting > floorTime)
                {
                    recovering = true;
                    timerRunning = false;
                    moveToValidNavPos();
                }

            } else
            {
                timerRunning = false;
            }
        }
    }

    private void moveToValidNavPos()
    {
        float x = transform.position.x;       
        float z = transform.position.z;

        if (x > -bounds.x && x < bounds.x && z > -bounds.z && z < bounds.z)
        {
            //Inside playing field. what happens for rail?
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(x, offset.y, z)));            
        }

        else if (x > bounds.x && z > -bounds.z && z < bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(bounds.x - offset.x, offset.y, z)));        

        else if (x < -bounds.x && z > -bounds.z && z < bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(-bounds.x + offset.x, offset.y, z)));        

        else if (x > -bounds.x && x < bounds.x && z > bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(x, offset.y, bounds.z - offset.z)));

        else if (x > -bounds.x && x < bounds.x && z < -bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(x, offset.y, -bounds.z + offset.z)));

        else if (x > bounds.x && z > bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(bounds.x - offset.x, offset.y, bounds.z - offset.z)));

        else if (x < -bounds.x && z > bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(-bounds.x + offset.x, offset.y, bounds.z - offset.z)));

        else if (x > bounds.x && z < -bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(bounds.x - offset.x, offset.y, -bounds.z + offset.z)));

        else if (x < -bounds.x && z < -bounds.z)
            StartCoroutine(moveToTargetPosAndTurnUp(new Vector3(-bounds.x + offset.x, offset.y, -bounds.z + offset.z)));


    }



    void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if (health != null)
        {
            float absVelocity = Mathf.Abs(Vector3.Magnitude(collision.relativeVelocity));
            if (absVelocity > damageMinSpeed)
            {
                health.ChangeValue(-absVelocity * fallingDamageMultiplier);
                print("damaged for " + -absVelocity * fallingDamageMultiplier + " with speed " + absVelocity);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        collisions--;        
    }

    private IEnumerator moveToTargetPosAndTurnUp(Vector3 targetPos)
    {
        Quaternion goalRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        rb.isKinematic = true;

        while(targetPos != transform.position || transform.rotation != goalRot)
        {
            //move towards position
            float step = recoveryMovementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            //rotate upwards
            step = rotationSpeed * 10 * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRot, step);

            yield return null;
        }

        recovering = false;
        agent.enabled = true;
        agent.isStopped = false;
        agent.destination = goal.position;
        oldPos = goal.position;
        rb.isKinematic = false;
    }

}