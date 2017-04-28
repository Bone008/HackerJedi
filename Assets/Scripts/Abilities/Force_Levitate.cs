using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Levitate : MonoBehaviour {
    HackerPlayer HackerScr;
	// Use this for initialization
	void Start () {
		//HackerScr setzen
	}
	
	// Update is called once per frame
	void Update () {
        if (HackerScr.getUltimateMode())
        {

        }
	}
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    public abstract AbilityType Type { get; }

    protected bool bothTriggersDown;
    protected bool bothGripsDown;

    protected virtual void OnTriggerDown() { }
    protected virtual void OnTriggerUp() { }
    protected virtual void OnGripDown() { }
    protected virtual void OnGripUp() { }

    public void SetTriggersDown(bool value)
    {
        bool wasDown = bothTriggersDown;
        bothTriggersDown = value;

        if (value && !wasDown)
            OnTriggerDown();
        else if (!value && wasDown)
            OnTriggerUp();
    }

    public void SetGripDown(bool value)
    {
        bool wasDown = bothGripsDown;
        bothGripsDown = value;

        if (value && !wasDown)
            OnGripDown();
        else if (!value && wasDown)
            OnGripUp();
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
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
}
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
 */