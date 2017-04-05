using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Force_Scr : MonoBehaviour {

    //Variablen
    private GameObject selPoint;
    private GameObject selection;

	// Use this for initialization
	void Start () {
        selPoint = gameObject.transform.FindChild("SelPoint").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        //Bei linker Maustaste auslösen
        if (Input.GetButtonDown("Fire1"))
        {
            force();
        }
        grabAndThrow();
	}
    public void force()
    {
        //Gegner (noch untagged) in Reichweite (5) werden erkannt
        Collider[] cols = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider col in cols)
        {
            if (col.tag == "Enemy")
            {
                //Hilfsvariablen
                Vector3 tmp1=col.gameObject.transform.position; //Gegnerposition
                Vector3 tmp2=gameObject.transform.position;     //Handposition
                float tmp4 = Vector3.Distance(tmp1, tmp2)+1;    //Distanz dazwischen
                Vector3 tmp3 = 50*Vector3.Normalize(new Vector3(tmp1.x - tmp2.x, tmp1.y - tmp2.y, tmp1.z - tmp2.z))/tmp4; //Gegnerposition relativ zur Hand
                
                //Setzen von Velocity (Effekt des Wegschleuderns)
                col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(tmp3.x, 10, tmp3.z);
                col.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2));
            }
        }
    }

    public void grabAndThrow()
    {
        //bei Klick wird mit Raycast Selection gesetzt (Klicküberprüfung in Update)(Noch mit Mousecontrol)
        //Selectionspunkt wird von Controller geparented
        if(!selection && Input.GetButtonDown ("Fire3")){
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	        RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                selPoint.transform.position = hit.collider.bounds.center;
                
                selection = hit.collider.gameObject;
                selection.GetComponent<Throwable_OBJ>().setGrabed();
                Debug.Log("Grabed!");
            }
        }

        //Ausgewählter Gegner folgt Selectionspunkt solange Knopf gedrückt ist
        if (selection&&selection.tag == "Enemy")
        {
            selection.GetComponent<Rigidbody>().MovePosition(selPoint.transform.position);
            if (Input.GetButtonDown("Fire2"))
            {
                selection.GetComponent<Throwable_OBJ>().setFree();
                selection = null;
                Debug.Log("Fallen gelassen!");
            }
        }
    }
}
