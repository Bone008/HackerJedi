using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedGruntEnemy : EnemyBase
{
    public float newTargetPosThreshhold = 1f;
    public float rotationSpeed = 3.5f;
    public float hitRange, stoppingDistance, floorTime;

    public GameObject explo;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isOnFloor;
    private float startTimeFloorContact = 0;

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

        isOnFloor = false;
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
        if (rb.velocity.sqrMagnitude < .1f)
        {
            if (isOnFloor && Time.time - startTimeFloorContact > floorTime)
            {
                //implement get up animation
                //for now:
                agent.enabled = true;
                agent.isStopped = false;
                agent.destination = goal.position;
                oldPos = goal.position;
            }
            //add handling of disabled enemy outside of standard grid (move towards grid and lift up...)
        }
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RoomBlock"))
        {
            isOnFloor = true;
            startTimeFloorContact = Time.time;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("RoomBlock"))
        {
            isOnFloor = false;
        }
    }

}