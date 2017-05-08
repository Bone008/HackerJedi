using UnityEngine;
using System.Collections;
using System;

public class UpgradeLaserPointer : MonoBehaviour
{

    public LayerMask uiLayerMask;
    private VolumetricLines.VolumetricLineBehavior line;

    private ILaserInteractable aimedAt = null;

    public AudioClip acceptSFX;
    public AudioClip mouserollSFX;

    private void Start()
    {
        line = GetComponent<VolumetricLines.VolumetricLineBehavior>();
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, float.PositiveInfinity, uiLayerMask.value))
        {
            var newTarget = hit.collider.GetComponentInParent<ILaserInteractable>();
            if (newTarget != aimedAt)
            {
                if (aimedAt != null)
                    aimedAt.SetHovered(false);
                newTarget.SetHovered(true);
                aimedAt = newTarget;
                line.EndPos = hit.distance * Vector3.forward;
                
                AudioSource.PlayClipAtPoint(mouserollSFX, this.transform.position, 1f);
                Debug.Log("aiming at " + aimedAt);
            }
        }
        else if(aimedAt != null)
        {
            aimedAt.SetHovered(false);
            aimedAt = null;
            line.EndPos = 100.0f * Vector3.forward;
        }
    }

    // called by input handlers
    public void Activate()
    {
        if (aimedAt != null)
        {
            AudioSource.PlayClipAtPoint(acceptSFX, this.transform.position, 1f);
            aimedAt.Activate();
        }
    }
}
