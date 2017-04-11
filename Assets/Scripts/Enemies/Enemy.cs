﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float newTargetPosThreshhold = 1;
    public float hitRange;
    public float fireDelay = 0.6f;

    public GameObject explo;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;
    private Gun gun;

    private Coroutine firingCoroutine = null;

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

        if(agent.isOnNavMesh)
            agent.destination = goal.position;

        // get gun component from children
        gun = GetComponentInChildren<Gun>();
        gun.layer = LayerMask.NameToLayer("Hacker");
    }

    void FixedUpdate()
    {
        if (agent.isOnNavMesh && Vector3.Distance(oldPos, goal.position) > newTargetPosThreshhold)
        {
            agent.destination = goal.position;
            oldPos = goal.position;
        }

        if (agent.velocity.x >= -0.05f && agent.velocity.x <= 0.05f
            && agent.velocity.z >= -0.1f && agent.velocity.z <= 0.1f)
        {
            transform.LookAt(goal);
        }
        

        // fire while in range
        if ((goal.position - transform.position).sqrMagnitude <= hitRange * hitRange)
        {
            if (firingCoroutine == null)
                firingCoroutine = StartCoroutine(FireWhenReady());
        }
        else if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    public void OnDeath()
    {
        Instantiate(explo, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private IEnumerator FireWhenReady()
    {
        while (true)
        {
            gun.FireOnce();
            yield return new WaitForSeconds(fireDelay);
        }
    }
}