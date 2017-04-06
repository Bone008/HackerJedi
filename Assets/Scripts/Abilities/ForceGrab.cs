using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ForceGrab : AbstractAbility {
    
    public override AbilityType Type { get { return AbilityType.JediLevitate; } }

    public Transform nozzle;
    private GameObject selection = null;

    protected override void OnTriggerDown()
    {
        if (!selection)
        {
            Ray ray = GetAimRay(nozzle);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //selPoint.transform.position = hit.collider.bounds.center;

                if (hit.collider.gameObject.tag == "Enemy")
                {
                    selection = hit.collider.gameObject;
                    selection.transform.SetParent(transform, true);
                    selection.GetComponent<Throwable_OBJ>().setGrabbed();
                    Debug.Log("Grabbed!");
                }
            }
        }
    }

    protected override void OnTriggerUp()
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
