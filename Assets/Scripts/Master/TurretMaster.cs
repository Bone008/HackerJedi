using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMaster : MonoBehaviour
{
    [HideInInspector]
    public Transform turretBarrel;
    private Gun gun;

    // cam rotation
    public float camRotYMin = -60.0f;
    public float camRotYMax = 60.0f;
    private float rotX, rotY;

    private void OnEnable()
    {
        gun = turretBarrel.GetComponentInChildren<Gun>();
    }

    void Update()
    {
        // rotate turret
        rotX += Input.GetAxis("Mouse X");
        rotY -= Input.GetAxis("Mouse Y");
        rotY = Mathf.Clamp(rotY, camRotYMin, camRotYMax);
        turretBarrel.localRotation = Quaternion.Euler(0, rotX, rotY);

        // single shot
        // TODO
        if (Util.InputGetAxisDown("Fire1") == true)
            gun.FireOnce();
    }
}
