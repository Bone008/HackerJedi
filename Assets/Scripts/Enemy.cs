using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float newTargetPosThreshhold = 1;
    public float hitRange;
    public float fireDelay = 0.6f;
    public float initialHealth = 100f;

    private Transform goal;
    private Vector3 oldPos;
    private NavMeshAgent agent;
    private Gun gun;
    private float currentHealth;

    private Coroutine firingCoroutine = null;

    void Start()
    {
        // locate player
        goal = GameObject.FindGameObjectWithTag("Player").transform;

        oldPos = goal.position;
        agent = GetComponent<NavMeshAgent>();

        if(agent.isOnNavMesh)
            agent.destination = goal.position;

        // get gun component from children
        gun = GetComponentInChildren<Gun>();
        gun.layer = LayerMask.NameToLayer("Hacker");

        // set current health
        currentHealth = initialHealth;
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

    public void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            Destroy(gameObject);
            return;
        }
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