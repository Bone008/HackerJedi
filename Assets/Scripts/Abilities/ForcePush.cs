using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : AbstractAbility {
    
    public override AbilityType Type { get { return AbilityType.JediForcePush; } }

    public float forceStrength;
    public float forceLift;
    public float range;

    private Vector3 PosePosBegin;
    private Vector3 PosePosEnd;

    private GameObject RelationObject; //Headcam-Object for example

    private void Start()
    {
        RelationObject = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnTriggerDown()
    {
        PosePosBegin = gameObject.transform.position;
    }

    protected override void OnTriggerUp()
    {
        PosePosEnd = gameObject.transform.position;

        Debug.Log("begin: " + Vector3.Distance(RelationObject.transform.position, PosePosBegin) + ", end: " + Vector3.Distance(RelationObject.transform.position, PosePosEnd));

        //Falls eine Bewegung vom Spieler weg gemacht wird, wird die Pose angenommen
        if (Vector3.Distance(RelationObject.transform.position, PosePosBegin) < Vector3.Distance(RelationObject.transform.position, PosePosEnd))
        {
            force(PosePosEnd - PosePosBegin);
        }
    }

    //Forceimpulse in every direction (targetDirection==Vector3.zero) or in a specified direction (targetDirection!=Vector3.zero)
    public void force(Vector3 targetDirection)
    {
        //Gegner (noch untagged) in Reichweite (5) werden erkannt
        Collider[] cols = Physics.OverlapSphere(transform.position, range);
        foreach (Collider col in cols)
        {
            if (col.tag == "Enemy")
            {
                //Hilfsvariablen
                Vector3 enemyPos = col.gameObject.transform.position; //Gegnerposition
                Vector3 handPos = gameObject.transform.position;     //Handposition
                float dist = Vector3.Distance(enemyPos, handPos);    //Distanz dazwischen
                Vector3 forceVec = forceStrength * Vector3.Normalize(enemyPos - handPos) * (1 - (dist / range) * (dist / range)); //Gegnerposition relativ zur Hand

                forceVec.y = forceLift;

                float angle = Vector3.Angle(targetDirection, forceVec);
                //Setzen von Velocity (Effekt des Wegschleuderns)
                if (dist < 0.5f || targetDirection == Vector3.zero || (angle < 40 + 30 / (dist+1.0f) && angle > -45))
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
