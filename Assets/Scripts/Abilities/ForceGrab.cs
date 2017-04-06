using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ForceGrab : AbstractAbility
{

    public override AbilityType Type { get { return AbilityType.JediLevitate; } }

    public Transform nozzle;
    private GameObject selection = null;

    private RaycastHit? GetAimedAtEnemy()
    {
        Ray ray = GetAimRay(nozzle);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.CompareTag("Enemy"))
            return hit;
        else
            return null;
    }

    void Update()
    {
        if (!isTriggerDown)
        {
            RaycastHit? hit = GetAimedAtEnemy();
            if (hit != null)
                Debug.Log("aiming at " + hit.Value.point, hit.Value.collider.gameObject);
        }
    }

    protected override void OnTriggerDown()
    {
        if (!selection)
        {
            RaycastHit? hit = GetAimedAtEnemy();

            if (hit == null)
                return;

            selection = hit.Value.collider.gameObject;
            selection.transform.SetParent(transform, true);
            selection.GetComponent<Throwable_OBJ>().setGrabbed();
            Debug.Log("Grabbed!");
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
