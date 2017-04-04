using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Master : MonoBehaviour {

    public Camera masterCamera;
    public Transform masterEye;
    public LayerMask raycastLayermask;

    public float rotationSpeed;

    [Header("Movement")]
    public Transform movementMin;
    public Transform movementMax;
    public float movementSpeed;
    public string movementInputAxis;

    [Header("Spawning")]
    public GameObject enemyPrefab;


    private bool createEnemyOnClick = false;

	void Start () {
        // for easier testing: create enemies from the start
        OnButtonEnemyCreate();
	}
	
	void Update () {
        // get aimed-for object via Raycast
        Transform objectHit = null;
        RaycastHit hit;
        Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayermask.value))
        {
            objectHit = hit.transform;
        }

        // left mouse button down
        if (objectHit != null && Input.GetMouseButtonDown(0) && createEnemyOnClick)
        {
            // spawn enemy
            Instantiate(enemyPrefab, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0));
            createEnemyOnClick = false;
        }

        // rotate eye to point to raycast hit
        if (objectHit)
        {
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - masterEye.position);
            masterEye.rotation = Quaternion.Slerp(masterEye.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // move eye
        float input = Input.GetAxis(movementInputAxis);
        if(input != 0)
        {
            var newPosition = transform.position + input * Vector3.forward * movementSpeed;
            newPosition.z = Mathf.Clamp(newPosition.z, movementMin.position.z, movementMax.position.z);
            transform.position = newPosition;
        }
    }

    public void OnButtonEnemyCreate()
    {
        createEnemyOnClick = true;        
    }


}
