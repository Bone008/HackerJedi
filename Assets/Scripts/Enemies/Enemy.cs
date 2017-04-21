using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float newTargetPosThreshhold = 1;
    public float hitRange, stoppingDistance;

    public GameObject explo;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;

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

        goal = player.transform;
        oldPos = goal.position;
        agent = GetComponent<NavMeshAgent>();

        if (agent.isOnNavMesh)
            agent.destination = goal.position;              
    }

    void Update()
    {
        //if (agent.isOnNavMesh)
        //{
            if (Vector3.Distance(transform.position, goal.position) < stoppingDistance /*&& !agent.isStopped*/)
            {
                agent.isStopped = true;
                //agent.destination = transform.position;
                agent.enabled = false;
            }

            if (Vector3.Distance(transform.position, goal.position) > hitRange /*&& agent.isStopped*/)
            {                
                agent.enabled = true;
                //agent.destination = goal.position;
                agent.isStopped = false;
            }

            if (Vector3.Distance(oldPos, goal.position) > newTargetPosThreshhold /*&& !agent.isStopped*/)
            {
                agent.destination = goal.position;
                oldPos = goal.position;
            }

            if (agent.velocity.x >= -0.05f && agent.velocity.x <= 0.05f
                && agent.velocity.z >= -0.1f && agent.velocity.z <= 0.1f)
            {
                transform.LookAt(goal);
            }
        //}
        Debug.Log(agent.isStopped);
        Debug.Log(Vector3.Distance(transform.position, goal.position));        
    }

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
}