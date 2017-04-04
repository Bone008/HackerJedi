using System.Collections;
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


    private Transform selected;

	void Start () {
	}
	
	void Update () {
        // left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            // get aimed-for object via Raycast, prevent onclick when pressing buton
            RaycastHit hit;
            Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayermask.value) && hit.transform.tag.Equals("RoomBlock"))
                selected = hit.transform;
            else if(!EventSystem.current.IsPointerOverGameObject())
                selected = null;
        }

        // move blocks
        float scrollAmount = -Input.GetAxis("Mouse ScrollWheel");
        if (selected && scrollAmount != 0)
        {
            Transform parent = selected.parent.transform;
            float y = parent.position.y + scrollAmount;
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
        if(input != 0)
        {
            var newPosition = transform.position + input * Vector3.forward * movementSpeed;
            newPosition.z = Mathf.Clamp(newPosition.z, movementMin.position.z, movementMax.position.z);
            transform.position = newPosition;
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
