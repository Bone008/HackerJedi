using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ToggleMasterCam : MonoBehaviour {

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Toggle Master Camera"))
            cam.enabled = !cam.enabled;
    }

}
