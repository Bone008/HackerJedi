using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Levitate : AbstractUltimate
{
    private float startMoveHeight;

    //public override AbilityType Type { get { return AbilityType.JediForcePush; } }//Abändern!!!

    /*protected override void OnGripsDown()
    {
        //Check if controllers are ([distance] below the headposition) and nearly rotated 90/-90 degrees around their forward vector
        
        //--> set UltimateActive (moveHeightStart=transform.position.y)
        //--> unselect selected Enemies
    }

    protected override void OnGripUp()
    {
        float moveHeightEnd = transform.position.y;
        //Check if the controllers are [distance] higher as the position before OnGripsDown()
        if (moveHeightEnd - startMoveHeight > 0.5)
        {
            //--> Do the Levitate! 
        }
        else
        {
            startMoveHeight = 100;
        }
        
    }

    public override bool CheckForStartpoint(Vector3 handLeft, Vector3 handRight)
    {
        //SetGripsDown(true);
        return true;
    }
  */  
}
    /*HackerPlayer HackerScr;
	// Use this for initialization
	void Start () {
		//HackerScr setzen
	}
	
	// Update is called once per frame
	void Update () {
        //if (HackerScr.getUltimateMode())
        //{

        //}
	}
    
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

    protected virtual void OnTriggersDown(Vector3 handLeft, Vector3 handRight) { }
    protected virtual void OnTriggerUp(Vector3 handLeft, Vector3 handRight) { }
    protected virtual void OnGripsDown(Vector3 handLeft, Vector3 handRight) { }
    protected virtual void OnGripUp(Vector3 handLeft, Vector3 handRight) { }
    //Checks if the positions of the controllers are possible Move-Startpoints
    public virtual bool CheckForStartpoint(Transform handLeft, Transform handRight) { return false; }

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

    //Should only be called when both grips are pressed (or one released) and the controllers are at the correct position (-> CheckForStartpoint())
    public bool SetGripsDown(bool value, Transform handLeft, Transform handRight)
    {

        bool wasDown = bothGripsDown;
        //bothGripsDown = value;(darf nur bei einem Wechsel gesetzt werden)

        if (value && !wasDown)
        {
            if (CheckForStartpoint(handLeft, handRight))
            {
                OnGripsDown(handLeft.position,handRight.position);
                bothGripsDown = value;
                return true;
            }
        }
        else if (!value && wasDown)
        {
            OnGripUp(handLeft.position, handRight.position);//Hier muss eventuell noch gecheckt werden, ob der Move auch correct war         !!!!!
            bothGripsDown = value;
            return true;
        }
        return false;
    }

    /// <summary>Simulates pressing and releasing the trigger instantaneously.</summary>
    public void FireOnce()
    {
        SetTriggersDown(true);
        SetTriggersDown(false);
    }


    /// <summary>Utility function for ability scripts to get a ray in the direction the hand is aiming</summary>
    public Ray GetAimRay(Transform nozzle = null)
    {
        Transform trans = nozzle ?? transform; // use own transform if no nozzle is defined

        var aimDirection = trans.TransformDirection(Vector3.forward).normalized;
        return new Ray(trans.position, aimDirection);
    }
 * 
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
