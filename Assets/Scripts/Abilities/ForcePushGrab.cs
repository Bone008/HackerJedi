﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePushGrab : AbstractAbility {
    
    public override AbilityType Type { get { return AbilityType.JediForcePush; } }


    // ==== Force push ====

    [Header("Force push")]
    public float forceStrength;
    public float forceLift;
    public float range;

    private Transform relationObject; //Headcam-Object for example
    private Vector3 posePosBegin; // in coordinate space of relationObject

    private void Start()
    {
        relationObject = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void OnTriggerDown()
    {
        posePosBegin = relationObject.InverseTransformPoint(transform.position);
    }

    protected override void OnTriggerUp()
    {
        Vector3 posePosEnd = relationObject.InverseTransformPoint(transform.position);
        Vector3 pushDirection = relationObject.TransformDirection(posePosEnd - posePosBegin);
        Vector3 origin = relationObject.TransformPoint(posePosBegin);

        Debug.Log("direction: " + pushDirection + "; len: " + pushDirection.magnitude);

        // if moved less than a threshold, don't accept (prevent accidental trigger presses)
        if (pushDirection.sqrMagnitude < 0.15f * 0.15f)
            return;

        // if the movement was towards the head, don't accept it
        //if (posePosBegin.sqrMagnitude > posePosEnd.sqrMagnitude)
        //    return;

        force(origin, pushDirection);
    }

    //Forceimpulse in every direction (targetDirection==Vector3.zero) or in a specified direction (targetDirection!=Vector3.zero)
    public void force(Vector3 origin, Vector3 targetDirection)
    {
        //Gegner (noch untagged) in Reichweite (5) werden erkannt
        Collider[] cols = Physics.OverlapSphere(origin, range);
        foreach (Collider col in cols)
        {
            if (col.tag == "Enemy")
            {
                //Hilfsvariablen
                Vector3 enemyPos = col.gameObject.transform.position; //Gegnerposition
                Vector3 handPos = gameObject.transform.position;     //Handposition
                float dist = Vector3.Distance(enemyPos, handPos);    //Distanz dazwischen

                Vector3 dir = (targetDirection == Vector3.zero ? Vector3.Normalize(enemyPos - handPos) : targetDirection.normalized);
                Vector3 forceVec = forceStrength * dir * (1 - (dist / range) * (dist / range)); //Gegnerposition relativ zur Hand

                forceVec.y = forceLift;

                float angle = Vector3.Angle(nozzle.forward, enemyPos - handPos);
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




    // ==== Force grab ====

    [Header("Force grab")]
    public Transform nozzle;
    public LineRenderer aimPreview;
    public Material aimPreviewStandardMaterial;
    public Material aimPreviewHighlightMaterial;
    /// <summary>How far off the aim is allowed to be to still register as a hit.</summary>
    public float grabRange;
    public float aimHitTolerance;
    private Throwable_OBJ selection = null;

    private Throwable_OBJ GetAimedAtTarget(out RaycastHit? hitOut)
    {
        Ray ray = GetAimRay(nozzle);

        RaycastHit hit;
        if (Physics.SphereCast(ray, aimHitTolerance, out hit, grabRange, LayerMask.GetMask("Enemies")))
        {
            var targetObj = hit.collider.GetComponentInParent<Throwable_OBJ>();
            if (targetObj != null)
            {
                hitOut = hit;
                return targetObj;
            }
        }

        hitOut = null;
        return null;
    }

    void Update()
    {
        if (selection == null)
        {
            RaycastHit? hit;
            Throwable_OBJ target = GetAimedAtTarget(out hit);
            
            aimPreview.enabled = true;
            aimPreview.SetPosition(0, nozzle.position);

            if (hit == null)
            {
                aimPreview.sharedMaterial = aimPreviewStandardMaterial;
                aimPreview.SetPosition(1, nozzle.TransformPoint(grabRange * Vector3.forward));
            }
            else
            {
                var collider = target.GetComponent<Collider>();
                Vector3 pos = (collider != null ? collider.bounds.center : target.transform.position);

                aimPreview.sharedMaterial = aimPreviewHighlightMaterial;
                aimPreview.SetPosition(1, pos);
                //Debug.Log("aiming at " + hit.Value.point, hit.Value.collider.gameObject);
            }
        }
        else if (aimPreview.enabled)
        {
            // disable preview while grabbing
            aimPreview.enabled = false;
        }
    }

    protected override void OnGripDown()
    {
        Debug.Log("Grip down!");
        if (selection == null)
        {
            RaycastHit? hit;
            Throwable_OBJ target = GetAimedAtTarget(out hit);

            if (hit == null)
                return;

            selection = target;
            selection.transform.SetParent(transform, true);
            selection.setGrabbed();
            Debug.Log("Grabbed!");
        }
    }

    protected override void OnGripUp()
    {
        Debug.Log("Grip up!");
        if (selection)
        {
            selection.setFree();
            selection.transform.SetParent(null);
            selection = null;
            Debug.Log("Fallen gelassen!");
        }
    }


}