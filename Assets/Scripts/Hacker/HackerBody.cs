using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackerBody : MonoBehaviour {

    private Transform hackerCam;

    private void Start()
    {
        hackerCam = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        transform.position = hackerCam.position;
    }
}
