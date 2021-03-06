﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Force_Levitate : AbstractUltimate
{
    private float startMoveHeightL;
    [Header("Ultimate force levitate")]
    public float forceStrengthL;
    public float rangeL;

    private float staffAlpha = 0;
    private Material staffMaterial;

    private void Start()
    {
        staffMaterial = staffTransform.GetComponentInChildren<Renderer>().material;
    }

    private void LateUpdate()
    {
        bool canPush = activated || CanStartForcePush();

        float alphaDir = (canPush ? 1 : -1);
        staffAlpha = Mathf.Clamp01(staffAlpha + alphaDir * Time.deltaTime * 3.0f);

        if(staffAlpha > 0)
        {
            staffTransform.gameObject.SetActive(true);
            staffTransform.position = (leftHand.position + rightHand.position) / 2;
            staffTransform.rotation = Quaternion.identity;
            staffMaterial.color = new Color(staffMaterial.color.r, staffMaterial.color.g, staffMaterial.color.b, Util.EaseInOut01(staffAlpha));
        }
        else
            staffTransform.gameObject.SetActive(false);
    }

    protected override void OnGripDown() {

        //Check if controllers are ([distance] below the headposition and) nearly rotated 90/-90 degrees around their forward vector
        if (!activated && Vector3.Angle(-leftHand.right, Vector3.down) <= 90 && Vector3.Angle(rightHand.right, Vector3.down) <= 90)
        {
            activated = true;
            startMoveHeightL = (leftHand.position.y + rightHand.position.y) / 2;
            //@HackerPlayer: Ultimate wurde erkannt und aktiviert
            EnableUlti();
            Debug.Log("Ultimate-Tracking started! #For_Lev OnGripDown()");
        }
        else
        {
            Debug.Log("Ultimate-Tracking not started! #For_Lev OnGripDown()");
        }
    }
    protected override void OnGripUp() {
        if (!activated)
            return;

        activated = false;
        DisableUlti();

        float moveHeightEnd = (leftHand.position.y + rightHand.position.y) / 2;
        //Check if the controllers are [distance] higher as the position before OnGripsDown()
        if (moveHeightEnd - startMoveHeightL > 0.3f)
        {
            Debug.Log("Ultimate-Tracking successfull! #For_Lev OnGripUp()");
            //--> Do the Levitate! 
            Collider[] cols = Physics.OverlapSphere(transform.position, rangeL);
            Throwable_OBJ[] behaviours = new Throwable_OBJ[cols.Length];
            int counter = 0;
            foreach (Collider col in cols)
            {
                if (col.tag == "Enemy")
                {
                    this.Delayed(0.1f, () => col.attachedRigidbody.AddRelativeForce(0, forceStrengthL, 0, ForceMode.Acceleration));
                    Throwable_OBJ obj=col.gameObject.GetComponent<Throwable_OBJ>();
                    obj.setGrabbed();//Enemy-Behaviours disablen?
                    behaviours[counter] = obj;
                }
            }
            ActivateEffect<Levitate_Reset>(10).init(behaviours);

            //DisableUlti();
        }
        else
        {
            Debug.Log("Ultimate-Tracking failed! #For_Lev OnGripUp()");
        }
    }
    //private bool testing = false;
    //private void TestMethod()
    //{
    //    if (testing)
    //    {
    //        return;
    //    }
    //    testing = true;
    //    Debug.LogWarning("Testcode is activated! #For_Lev LateUpdate()");
    //    //--> Do the Levitate! 
    //    Collider[] cols = Physics.OverlapSphere(transform.position, rangeL);
    //    Throwable_OBJ[] behaviours = new Throwable_OBJ[cols.Length];
    //    int counter = 0;
    //    foreach (Collider col in cols)
    //    {
    //        if (col.tag == "Enemy")
    //        {
    //            this.Delayed(0.1f, () => col.attachedRigidbody.AddRelativeForce(0, forceStrengthL, 0, ForceMode.Acceleration));
    //            Throwable_OBJ obj = col.gameObject.GetComponent<Throwable_OBJ>();
    //            obj.setGrabbed();//Enemy-Behaviours disablen?
    //            behaviours[counter] = obj;
    //        }
    //        counter++;
    //    }
    //    ActivateEffect<Levitate_Reset>(15).init(behaviours);
    //}

    //--------------------------------------------------------------------------------------------------------------------------------------------------------
    //Shockwave-Ultimate
    //--------------------------------------------------------------------------------------------------------------------------------------------------------
    private float startMoveHeightG;
    [Header("Ultimate force push")]
    public Transform staffTransform;
    public GameObject shockwavePrefab;
    public float forceStrengthG;
    public float forceLiftG;
    public float rangeG;

    private bool CanStartForcePush()
    {
        return !activated && Vector2.Distance(
            new Vector2(leftHand.position.x, leftHand.position.z),
            new Vector2(rightHand.position.x, rightHand.position.z)
            ) < 0.1f;
    }

    protected override void OnTriggerDown()
    {
        //Check if controllers are [distance] over the headposition and one higher than the other
        if (CanStartForcePush())
        {
            activated = true;
            startMoveHeightG = (leftHand.position.y + rightHand.position.y) / 2;
            //@HackerPlayer: Ultimate wurde erkannt und aktiviert
            EnableUlti();
            Debug.Log("Ultimate-Tracking started! #For_Gun OnTriggerDown()");
        }
        else
        {
            Debug.Log("Ultimate-Tracking failed! #For_Gun OnTriggerDown()");
        }
    }
    protected override void OnTriggerUp()
    {
        float moveHeightEnd = (leftHand.position.y + rightHand.position.y) / 2;
        //Check if the controllers are [distance] higher as the position before OnGripsDown()
        if (activated && moveHeightEnd - startMoveHeightG < -0.3f)
        {
            Debug.Log("Ultimate-Tracking successfull! #For_Gun OnTriggerUp()");
            //--> Do Force push! 
            force();

            // shockwave
            var shockwavePos = staffTransform.position;
            var platform = GameObject.FindGameObjectWithTag("Platform");
            if(platform != null)
                shockwavePos.y = platform.transform.position.y + 0.2f;
            Instantiate(shockwavePrefab, shockwavePos, Quaternion.identity);
        }
        else
        {
            Debug.Log("Ultimate-Tracking failed! #For_Gun OnTriggerUp()");
        }
        DisableUlti();
        activated = false;
    }

    private void force()
    {
        //Gegner in Reichweite werden erkannt
        Collider[] cols = Physics.OverlapSphere(transform.position, rangeG);//Kann noch abhängig von Handposition gemacht werden
        foreach (Collider col in cols)
        {
            if (col.tag == "Enemy")
            {
                var nma = col.GetComponentInParent<NavMeshAgent>();
                if (nma != null)
                    nma.enabled = false;
                //Hilfsvariablen
                Vector3 enemyPos = col.gameObject.transform.position; //Gegnerposition
                Vector3 handPos = gameObject.transform.position;     //Handposition
                float dist = Vector3.Distance(enemyPos, handPos);    //Distanz dazwischen

                Vector3 dir = Vector3.Normalize(enemyPos - handPos);
                Vector3 forceVec = forceStrengthG * dir * (1 - (dist / rangeG) * (dist / rangeG)); //Gegnerposition relativ zur Hand

                forceVec.y = forceLiftG;

                //Setzen von Velocity (Effekt des Wegschleuderns)
                Debug.Log("affecting enemy " + forceVec, col.gameObject);

                Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(forceVec, ForceMode.Impulse);
                rb.AddTorque(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2)), ForceMode.Impulse);

            }
        }
    }
}