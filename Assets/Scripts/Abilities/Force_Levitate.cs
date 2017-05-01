using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force_Levitate : AbstractUltimate
{
    private float startMoveHeight;

    public override AbilityType Type { get { return AbilityType.JediForcePush; } }//Abändern!!!

    protected override void OnGripsDown()
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
        SetGripsDown(true);
        return true;
    }
    
}
    /*HackerPlayer HackerScr;
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
 */