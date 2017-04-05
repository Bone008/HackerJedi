﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Master : MonoBehaviour {

    public Camera masterCamera;
    public Transform masterEye;
    public LayerMask raycastLayermask;

    [Header("Movement")]
    public Transform movementMin;
    public Transform movementMax;
    public float movementSpeed;
    public string movementInputAxis;

    [Header("Spawning")]
    public GameObject enemyPrefab;

    [Header("Block Moving")]
    public float blockMinYValue = 0;
    public float blockMaxYValue = 6;
    public float blockSpeed = 30.0f;
    private bool currentlyDragging;
    private float mouseDragStartY;

    [Header("Laser Beam")]
    public LineRenderer laserBeam;
    public Transform laserStart;
    private LookAtMouse lookAtMouseScript;

    private Transform selected;
    
	void Start () {
        laserBeam.enabled = false;
        lookAtMouseScript = ((LookAtMouse)masterEye.GetComponent("LookAtMouse"));
    }

    void Update()
    {
        // select cube and start dragging
        if (Input.GetMouseButtonDown(0))
        {
            // get aimed-for object via Raycast, prevent onclick when pressing buton
            RaycastHit hit;
            Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayermask.value) && hit.transform.tag.Equals("RoomBlock"))
            {
                selected = hit.transform;
                currentlyDragging = true;
            }
            else if (!EventSystem.current.IsPointerOverGameObject()) // if no button was pressed
            {
                selected = null;
            }
        }

        // stop dragging
        if (Input.GetMouseButtonUp(0))
        {
            currentlyDragging = false;
        }

        // move blocks
        float mouseDragDiff = blockSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        if (selected && currentlyDragging && (mouseDragDiff > 0.0001f || -0.0001f > mouseDragDiff))
        {
            Transform parent = selected.parent.transform;
            float y = parent.position.y + mouseDragDiff;
            y = Mathf.Max(blockMinYValue, y);
            y = Mathf.Min(blockMaxYValue, y);
            parent.position = new Vector3(
                parent.position.x,
                y,
                parent.position.z
            );
        }

        // move master
        float input = Input.GetAxis(movementInputAxis);
        if (input != 0)
        {
            var newPosition = transform.position + input * Vector3.forward * movementSpeed;
            newPosition.z = Mathf.Clamp(newPosition.z, movementMin.position.z, movementMax.position.z);
            transform.position = newPosition;
        }

        // move laser
        if (selected != null && currentlyDragging)
        {
            laserBeam.SetPosition(0, laserStart.position);
            laserBeam.SetPosition(1, selected.position);
            if(!laserBeam.enabled)
            {
                laserBeam.enabled = true;
                lookAtMouseScript.rotationSpeed *= 10.0f;
            }            
        }
        else
        {
            if(laserBeam.enabled)
            {
                laserBeam.enabled = false;
                lookAtMouseScript.rotationSpeed /= 10.0f;
            }            
        }
    }

    public void OnButtonEnemyCreate()
    {
        // spawn enemy
        if (selected != null)
        {
            // TODO this gets done by the navagent
            var enemyCollider = enemyPrefab.GetComponent<Collider>();
            float offsetY = 0;
            if (enemyCollider != null)
                offsetY = -enemyCollider.bounds.min.y;

            Instantiate(enemyPrefab, selected.position + Vector3.up * offsetY, Quaternion.Euler(0, Random.Range(0, 360), 0));
        }
    }
    
}
