using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Valve.VR.InteractionSystem
{
    //for throwing the enemy
    [RequireComponent(typeof(VelocityEstimator))]
    [RequireComponent(typeof(Rigidbody))]
    //end

    public class Enemy : MonoBehaviour
    {
        public float newTargetPosThreshhold, stopRange;
        public float hitRange;
        public float fireDelay = 0.6f;

        private Transform goal;
        private Vector3 oldPos;
        private NavMeshAgent agent;
        private bool following = true;
        private Gun gun;

        private Coroutine firingCoroutine = null;
        private VelocityEstimator velocityEstimator; //for throwing the enemy

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

        void Awake()
        {
            velocityEstimator = GetComponent<VelocityEstimator>();
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

        private IEnumerator FireWhenReady()
        {
            while (true)
            {
                gun.Fire();
                yield return new WaitForSeconds(fireDelay);
            }
        }
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Throwing-Functionality
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void setGrabed()
        {
            velocityEstimator.BeginEstimatingVelocity();
            //evtl bool grabed? For Movementdeaktivation (grabed=true)
        }

        public void setFree()
        {
            velocityEstimator.FinishEstimatingVelocity();
            gameObject.GetComponent<Rigidbody>().velocity = velocityEstimator.GetVelocityEstimate();
            gameObject.GetComponent<Rigidbody>().position = velocityEstimator.transform.position;
            //(grabed=false)
        }
    }
}