﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedGruntEnemy : EnemyBase
{
    public float newTargetPosThreshhold = 1f;
    public float rotationSpeed = 5f;
    public Vector3 offset = new Vector3(.5f, 1.533333f, .5f);
    public float hitRange, stoppingDistance, floorTime, recoveryMovementSpeed;
    public Vector3 bounds;

    public GameObject explo;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool recovering, timerRunning;
    private float startTimeResting = 0;
    private int collisions = 0;

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
       
        recovering = false;
        timerRunning = false;
        goal = player.transform;
        oldPos = goal.position;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;

        if (agent.isOnNavMesh)
            agent.destination = goal.position;              
    }

    void Update()
    {
        
        if (agent.enabled)
        {
            //Handle enemy in functional state and on navmesh
            float dist = Vector3.Distance(transform.position, goal.position);
            if (agent.isOnNavMesh)
            {
                handleFunctionalEnemy(dist);
            }
        }
        else
        {
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
        Destroy(gameObject);
    }

    private void handleFunctionalEnemy(float dist)
    {
        //Stop enemy if close enough
        if (dist < stoppingDistance && !agent.isStopped)
        {
            agent.isStopped = true;
            //agent.destination = transform.position;
            //agent.enabled = false;                
        }

        //follow player if distance too big
        if (dist > hitRange && agent.isStopped)
        {
            //agent.enabled = true;
            //agent.destination = goal.position;
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
            //transform.LookAt(new Vector3(goal.position.x, transform.position.y, goal.position.z));
            rotateTowards(goal);
        }
        else
        {
            agent.updateRotation = true;
        }
        //Debug.Log("onNavMesh loop " + Time.time + " " + agent.isOnNavMesh);
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
                    //implement get up animation
                    //for now:
                    //agent.enabled = true;
                    //agent.isStopped = false;
                    //agent.destination = goal.position;
                    //oldPos = goal.position;
                }
                //add handling of disabled enemy outside of standard grid (move towards grid and lift up...)
                //moveToValidNavPos();
            } else
            {
                timerRunning = false;
            }
        }
        else
        {
            //is there even any "else"?
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
    }

    private void OnCollisionExit(Collision collision)
    {
        collisions--;        
    }

    private IEnumerator moveToTargetPosAndTurnUp(Vector3 targetPos)
    {
        Transform initTrans = this.transform;
        float initDist = Vector3.Distance(initTrans.position, targetPos);
        Quaternion goalRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        Debug.Log("entered coroutine " + Time.time);
        rb.isKinematic = true;

        while(targetPos != transform.position || transform.rotation != goalRot)
        {
            Debug.Log(Time.time);
            //move towards position
            float step = recoveryMovementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            //rotate upwards
            //Quaternion rot = Quaternion.Slerp(initTrans.rotation, goalRot, Vector3.Distance(transform.position, targetPos)/initDist);
            step = rotationSpeed * 10 * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRot, step);
            //transform.rotation = Quaternion.Lerp(initTrans.rotation, goalRot, 1 - (Vector3.Distance(transform.position, targetPos) / initDist));

            yield return null;
        }

        Debug.Log("exited coroutine loop " + Time.time);
        recovering = false;
        agent.enabled = true;
        agent.isStopped = false;
        agent.destination = goal.position;
        oldPos = goal.position;
        rb.isKinematic = false;
    }

}