using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    //for throwing the enemy
    [RequireComponent(typeof(VelocityEstimator))]
    [RequireComponent(typeof(Rigidbody))]
    //end

    public class Throwable_OBJ : MonoBehaviour
    {

        private VelocityEstimator velocityEstimator; //for throwing the enemy

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void Awake()
        {
            velocityEstimator = GetComponent<VelocityEstimator>();
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
