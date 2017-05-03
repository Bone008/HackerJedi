using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Levitate : AbstractUltimate
{
    private float startMoveHeight;
    [Header("Ultimate force levitate")]
    public float forceStrength;
    public float range;

    protected override void OnGripDown() {

        //Check if controllers are ([distance] below the headposition and) nearly rotated 90/-90 degrees around their forward vector
        if (!activated && Vector3.Angle(-leftHand.right, Vector3.down) <= 90 && Vector3.Angle(rightHand.right, Vector3.down) <= 90)
        {
            activated = true;
            startMoveHeight = (leftHand.position.y + rightHand.position.y) / 2;
            //@HackerPlayer: Ultimate wurde erkannt und aktiviert
            EnableUlti();
            Debug.Log("Ultimate-Tracking started! #For_Lev OnGripDown()");
        }
        else
        {
            //@HackerPlayer: Ultimate wurde abgebrochen. Ein normaler Move soll ausgeführt werden
            Debug.Log("Ultimate-Tracking not started! #For_Lev OnGripDown()");
            //Eventuell nun nichtmehr nötig
        }
        //TODO:
        //--> set UltimateActive
        //--> unselect selected Enemies
    }
    protected override void OnGripUp() {

        float moveHeightEnd = (leftHand.position.y + rightHand.position.y) / 2;
        //Check if the controllers are [distance] higher as the position before OnGripsDown()
        if (activated && moveHeightEnd - startMoveHeight > 0.5)
        {
            Debug.Log("Ultimate-Tracking successfull! #For_Lev OnGripUp()");
            //--> Do the Levitate! 
            Collider[] cols = Physics.OverlapSphere(transform.position, range);
            Throwable_OBJ[] behaviours = new Throwable_OBJ[cols.Length];
            int counter = 0;
            foreach (Collider col in cols)
            {
                if (col.tag == "Enemy")
                {
                    col.attachedRigidbody.AddForce(0, forceStrength, 0, ForceMode.Acceleration);
                    Throwable_OBJ obj=col.gameObject.GetComponent<Throwable_OBJ>();
                    obj.setGrabbed();//Enemy-Behaviours disablen?
                    behaviours[counter] = obj;
                }
            }
            ActivateEffect<Force_Levitate>(10).StartCoroutine(Reset(behaviours));

            DisableUlti();
        }
        else
        {
            Debug.Log("Ultimate-Tracking failed! #For_Lev OnGripUp()");
        }
        activated = false;
    }
    IEnumerator Reset(Throwable_OBJ[] levitated)
    {
        yield return new WaitForSeconds(9);
        foreach (Throwable_OBJ col in levitated)
        {
            col.setFree();
        }
    }
}
    /*
    
    private void searchGestures()
    {
        //Für Gd Move muss dauerhaft überprüft werden, wo die Controller sind... Nicht besonders performant. Da sollten wir uns was anderes überlegen.
        
        //In OnTriggerUp//Wenn an beiden Seiten Force ausgewählt - Mit ultimateEnabled()
        //Dann:
            //Check auf Geste
                //wenn UltimateActive && (controler1Anfang.y - controller1Ende.y)>0.5 && (controler2Anfang.y - controller2Ende.y)>0.5
                //--> Gandalf Move Ausführen force(position, Vector3.zero);
                //Vorschau muss anders gehandled werden

                //wenn (controler1Anfang.y - controller1Ende.y)<0.5 && (controler2Anfang.y - controller2Ende.y)<0.5
                //--> Force-Levitate allen Gegnern constantforce 10 auf y geben

                //wenn (controller1 und 2 höher als Kopf und nah am Kopf)
            //Zugriff wie bei Forcepush
            //return false dass normal fortgesetzt wird
        //Sonst:
            //return true dass normal fortgesetzt wird
        
    }
}*/
//Noch direkt in Forcepush einbauen?
/*
 In Hackerplayer:
 * wenn UltimateActive() und beide Controller getriggert sind
 * -> Setze Ultimate auf active
 * wenn Ultimate active 
 * -> bei loslassen der beiden Trigger: Check ob Geste passt
 * (was ist falls nur einer losgelassen wird?)
 * wenn Geste passt
 * -> Move und retetUltimateMode
 * 
 * protected bool bothTriggersDown;
    protected bool bothGripsDown;

    //Should only be called when both triggers are pressed (or one released) and the controllers are at the correct position (-> CheckForStartpoint())
    public bool SetTriggersDown(bool value, Transform handLeft, Transform handRight)
    {
        bool wasDown = bothTriggersDown;
        //bothTriggersDown = value;

        if (value && !wasDown)
        {
            if (CheckForStartpoint(handLeft, handRight))
            {
                OnTriggersDown(handLeft.position, handRight.position);
                bothTriggersDown = value;
                return true;
            }
        }
        else if (!value && wasDown)
        {
            OnTriggerUp(handLeft.position, handRight.position);//Hier muss eventuell noch gecheckt werden, ob der Move auch correct war         !!!!!
            bothTriggersDown = value;
            return true;
        }
        return false;
    }
 */
/*Ablauf:
 * 1. HackerPlayer registriert Drücken von Trigger/Grip                     (Input -> HackerPlayer)
 * 2. HackerPlayer ermittelt, ob Ultimate aufgeladen und möglich ist        (HackerPlayer)
 * 3.1 Falls ja: HackerPlayer fragt HandPos ab und schickt es an Ultimate   (Ultimate: SetTriggers/Grips)
 * 3.2 Falls Nein: HackerPlayer setzt normalen Move um                      (Ability)
 * 4. Ultimate Checkt, ob Position und Rotation stimmen                     (Ultimate: SetTrigger/Grips -> CheckPosRot)
 * 5.1 Stimmt: Return true und ggf OnTriggerDown                            (SetTrigger ~> HackerPlayer)
 * 5.2 Nicht: Return false, Hackerplayer bricht Ultimate ab                 (SetTrigger -> HackerPlayer)
 * 6. On...Down speichert Positionen                                        (On...Down)
 * 7. Bei Loslassen eines Buttons: Check auf Ultimateabbruch                (
 */
