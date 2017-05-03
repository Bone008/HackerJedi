using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gundalf_U : AbstractUltimate {

    private float startMoveHeight;
    [Header("Ultimate force push")]
    public float forceStrength;
    public float forceLift;
    public float range;

    protected override void OnTriggerDown()
    {
        //Check if controllers are [distance] over the headposition and one higher than the other
        if (!activated && Vector2.Distance(new Vector2(leftHand.position.x,leftHand.position.z),new Vector2(rightHand.position.x,rightHand.position.z))<0.2)//Headposition und Vergleich fehlt hier noch!!!!
        {
            activated = true;
            startMoveHeight = (leftHand.position.y + rightHand.position.y) / 2;
            //@HackerPlayer: Ultimate wurde erkannt und aktiviert
            EnableUlti();
            Debug.Log("Ultimate-Tracking started! #For_Gun OnTriggerDown()");
        }
        else
        {
            //@HackerPlayer: Ultimate wurde abgebrochen. Ein normaler Move soll ausgeführt werden
            Debug.Log("Ultimate-Tracking failed! #For_Gun OnTriggerDown()");
            //Kann evtl entfernt werden
        }
        //TODO:
        //--> set UltimateActive
        //--> unselect selected Enemies
    }
    protected override void OnTriggerUp()
    {
        float moveHeightEnd = (leftHand.position.y + rightHand.position.y) / 2;
        //Check if the controllers are [distance] higher as the position before OnGripsDown()
        if (activated && moveHeightEnd - startMoveHeight < 0.5)
        {
            Debug.Log("Ultimate-Tracking successfull! #For_Gun OnTriggerUp()");
            //--> Do Force push! 
            
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
        //Gegner in Reichweite (5) werden erkannt
        Collider[] cols = Physics.OverlapSphere(transform.position, range);//Kann noch abhängig von Handposition gemacht werden
        foreach (Collider col in cols)
        {
            if (col.tag == "Enemy")
            {
                //Hilfsvariablen
                Vector3 enemyPos = col.gameObject.transform.position; //Gegnerposition
                Vector3 handPos = gameObject.transform.position;     //Handposition
                float dist = Vector3.Distance(enemyPos, handPos);    //Distanz dazwischen

                Vector3 dir = Vector3.Normalize(enemyPos - handPos);
                Vector3 forceVec = forceStrength * dir * (1 - (dist / range) * (dist / range)); //Gegnerposition relativ zur Hand

                forceVec.y = forceLift;

                //Setzen von Velocity (Effekt des Wegschleuderns)
                if (dist < 0.5f)
                {
                    Debug.Log("affecting enemy " + forceVec, col.gameObject);

                    Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
                    rb.AddForce(forceVec, ForceMode.Impulse);
                    rb.AddTorque(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2)), ForceMode.Impulse);
                }
            }
        }
    }
}
