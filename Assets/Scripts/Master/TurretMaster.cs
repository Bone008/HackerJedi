using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMaster : MonoBehaviour
{
    [HideInInspector]
    public Transform turret;
    public Transform turretBarrel;
    private Gun gun;

    // cam rotation
    public float camRotYMin = -60.0f;
    public float camRotYMax = 60.0f;
    private float rotX, rotY;

    private void OnEnable()
    {
        gun = turret.GetComponentInChildren<Gun>();
        gun.layer = LayerMask.NameToLayer("Hacker");
    }

    void Update()
    {
        // rotate turret
        rotX += Input.GetAxis("Mouse X") * Time.unscaledDeltaTime * 200.0f;
        rotY -= Input.GetAxis("Mouse Y") * Time.unscaledDeltaTime * 100.0f;
        rotY = Mathf.Clamp(rotY, camRotYMin, camRotYMax);
        turret.localRotation = Quaternion.Euler(0, rotX, 0);
        turretBarrel.localRotation = Quaternion.Euler(rotY, 0, 0);

        // single shot
        // TODO
        if (Util.InputGetAxisDown("Fire1") == true)
            gun.FireOnce();
    }
}
