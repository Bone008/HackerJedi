using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float newTargetPosThreshhold, stopRange;
    public float hitRange;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;
    private bool following = true;
    private Gun gun;


    void Start()
    {
        // locate player
        goal = GameObject.FindGameObjectWithTag("Player").transform;

        oldPos = goal.position;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;

        // get gun component from children
        gun = GetComponentInChildren<Gun>();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, goal.transform.position) > stopRange)
        {
            if (Vector3.Distance(oldPos, goal.position) > newTargetPosThreshhold)
            {
                agent.destination = goal.position;
                oldPos = goal.position;
            }
            following = true;
        }
        else
        {
            if (following)
            {
                Debug.Log("telling him to stop");
                Debug.Log(Vector3.Distance(transform.position, goal.transform.position));
                agent.destination = transform.position;
                following = false;
            }
        }

        // fire if in range
        if ((goal.position - transform.position).sqrMagnitude <= hitRange * hitRange)
        {
            gun.Fire();
        }
    }
}
