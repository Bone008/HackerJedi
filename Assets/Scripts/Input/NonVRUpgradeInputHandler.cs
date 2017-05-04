using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonVRUpgradeInputHandler : MonoBehaviour {

    public UpgradeLaserPointer laserPointer;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            laserPointer.Activate();
    }
}
