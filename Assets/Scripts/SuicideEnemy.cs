using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuicideEnemy : MonoBehaviour {
    public float newTargetPosThreshhold = 1;
    public float blinkRange;
    public float hitRange;
    public float initialHealth = 100f;
    private Transform goal;
    public Color color1;
    public Color color2;

    private Vector3 oldPos;
    private NavMeshAgent agent;
    private float currentHealth;

    private Coroutine blinkCoroutine;

    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Platform").transform;
        oldPos = goal.position;
        agent = GetComponent<NavMeshAgent>();

        if (agent.isOnNavMesh)
            agent.destination = goal.position;

        // set current health
        currentHealth = initialHealth;

        GetComponent<Renderer>().material.color = color1;
    }

    void FixedUpdate()
    {
        if (agent.isOnNavMesh && Vector3.Distance(oldPos, goal.position) > newTargetPosThreshhold)
        {
            agent.destination = goal.position;
            oldPos = goal.position;
        }


        float sqDist = (goal.position - transform.position).sqrMagnitude;
        if (sqDist <= blinkRange * blinkRange)
        {
            if (blinkCoroutine == null)
                blinkCoroutine = StartCoroutine(StartBlinking());

            // fire while in range
            if (sqDist <= hitRange * hitRange)
            {
                StopCoroutine(blinkCoroutine);
                goal.GetComponent<Platform>().DisableForSec(2.0f);
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            if(blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
        }
    }

    public void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            Destroy(gameObject);
            return;
        }
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
