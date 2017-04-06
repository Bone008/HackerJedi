using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Force_Scr : MonoBehaviour {

    //Variablen
    public GameObject selPoint;
    private GameObject selection;
    private Vector3 PosePosBegin;
    private Vector3 PosePosEnd;
    public GameObject RelationObject; //Headcam-Object for example

	// Use this for initialization
	void Start () {
        //selPoint = gameObject.transform.FindChild("SelPoint").gameObject;
	}
	
	// Update is called once per frame
	void Update () {

        //Testcode
        if (Input.GetButtonDown("Fire1"))
        {
            //force(Vector3.zero);//Shockwave "Gandalf"
            force(Vector3.forward);//Machtschlag
        }
        if (Input.GetButtonDown("Fire2"))
        {
            grabIt();
        }
        if (Input.GetButtonDown("Fire3"))
        {
            throwIt();
        }
        gameObject.transform.Rotate(new Vector3(2, 0, 0));
        //Ende Testcode

        //hold/move Item
        //if(selection){
        //    selection.GetComponent<Rigidbody>().MovePosition(selPoint.transform.position);
        //}
	}

//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Force-Wafe//
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void startMove()
    {
        PosePosBegin = gameObject.transform.position;
    }

    public void endMove()
    {
        PosePosEnd = gameObject.transform.position;
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
        Collider[] cols = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider col in cols)
        {
            if (col.tag == "Enemy")
            {
                //Hilfsvariablen
                Vector3 tmp1 = col.gameObject.transform.position; //Gegnerposition
                Vector3 tmp2 = gameObject.transform.position;     //Handposition
                float tmp4 = Vector3.Distance(tmp1, tmp2) + 1;    //Distanz dazwischen
                float tmp5;
                Vector3 tmp3 = 50 * Vector3.Normalize(new Vector3(tmp1.x - tmp2.x, tmp1.y - tmp2.y, tmp1.z - tmp2.z)) / tmp4; //Gegnerposition relativ zur Hand
                
                //Setzen von Velocity (Effekt des Wegschleuderns)
                if (tmp4<1.5f||targetDirection == Vector3.zero||((tmp5 = Vector3.Angle(targetDirection, tmp3)) < 40+30/tmp4&& tmp5>-45))
                {
                    col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(tmp3.x, 10, tmp3.z);
                    col.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2));
                }
            }
        }
    }

//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Force-Grab-Functionality//
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //bei Aufruf wird mit Raycast Selection gesetzt
    //Selectionspunkt wird von Controller geparented
    //Es muss sichergestellt sein, dass jedes Objekt mit tag "Enemy" auch throwable ist
    public bool grabIt()//Grabs an object if possible and returns, if object has been grabed
    {
        if (!selection)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //selPoint.transform.position = hit.collider.bounds.center;

                if (hit.collider.gameObject.tag == "Enemy")
                {
                    selection = hit.collider.gameObject;
                    selection.transform.SetParent(transform);//Test
                    selection.GetComponent<Throwable_OBJ>().setGrabbed();
                    Debug.Log("Grabed!");
                }
            }
        }
        if (selection)
        {
            return true;
        }
        return false;
    }

    //Bei Aufruf wird der gehaltene Gegenstand fallengelassen
    //Bei Bewegung wird er geworfen
    public void throwIt()
    {
        if (selection)
        {
            selection.GetComponent<Throwable_OBJ>().setFree();
            selection.transform.SetParent(null);
            selection = null;
            Debug.Log("Fallen gelassen!");
        }
    }
}
