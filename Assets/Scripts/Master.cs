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
    public float maxMovementDelta;
    public string movementInputAxis;

    [Header("Spawning")]
    private GameObject enemyPrefab = null;
    private GameObject obstaclePrefab = null;
    public LevelGenerator level;

    [Header("Block Moving")]
    public float blockMinYValue = 0;
    public float blockMaxYValue = 6;
    public float blockSpeed = 30.0f;
    private bool currentlyDragging;
    private Vector2 oldMousePosition;

    [Header("Block Snapping To Grid")]
    public float snappingSpeed = 1.0f;
    public float snappingGround = 0.0f;
    public float snappingGridSize = 2.0f;
    private bool currentlySnappingToGrid;
    private float targetY;

    [Header("Laser Beam")]
    public LineRenderer laserBeam;
    public Transform laserStart;
    private LookAtMouse lookAtMouseScript;

    private Transform selected;
    
	void Start () {
        laserBeam.enabled = false;
        lookAtMouseScript = masterEye.GetComponent<LookAtMouse>();
        oldMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // select cube
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            selected = null;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // get aimed-for object via Raycast, prevent onclick when pressing buton
                RaycastHit hit;
                Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayermask.value))
                {
                    selected = hit.transform;
                }
            }
        }

        // spawn enemy
        if (selected != null && Input.GetMouseButtonDown(0))
        {
            if (selected.tag.Equals("RoomBlock"))
                CreateEnemyAbove(selected.gameObject);
            else if (selected.tag.Equals("RailBlock"))
                CreateObstacleOn(selected);
        }

        // start dragging
        if (Input.GetMouseButtonDown(1) && selected != null)
        {
            currentlyDragging = true;
            currentlySnappingToGrid = false;
        }

        // stop dragging
        if (Input.GetMouseButtonUp(1))
        {
            currentlyDragging = false;
            currentlySnappingToGrid = true;

            // determine where to move to clip to grid
            float currentY = selected.parent.transform.position.y;
            int currentGridIndex = Mathf.FloorToInt((currentY - snappingGround) / snappingGridSize);
            float midY = snappingGround + (currentGridIndex * snappingGridSize) + (snappingGridSize / 2.0f);
            
            if(currentY < midY)
                targetY = snappingGround + (currentGridIndex * snappingGridSize);
            else
                targetY = snappingGround + ((currentGridIndex + 1) * snappingGridSize);
        }

        // move blocks
        Vector2 newMousePosition = Input.mousePosition;
        float mouseDragDiff = blockSpeed * (newMousePosition.y - oldMousePosition.y) * Time.deltaTime;
        if (selected && currentlyDragging)        {
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
        oldMousePosition = newMousePosition;

        // snap blocks to grid
        if (selected && currentlySnappingToGrid)
        {
            Vector3 pos = selected.parent.transform.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, Time.deltaTime * snappingSpeed);
            selected.parent.transform.position = pos;

            if (selected.parent.transform.position.y == targetY)
                currentlySnappingToGrid = false;
        }

        // move master
        float input = Input.GetAxis(movementInputAxis);
        if (input != 0)
        {
            Transform target = input < 0 ? movementMin : movementMax;
            transform.position = Vector3.MoveTowards(transform.position, target.position, maxMovementDelta);
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

    public void SelectEnemy(GameObject enemyPrefab)
    {
        this.enemyPrefab = enemyPrefab;
    }

    public void SelectObstacle(GameObject obstaclePrefab)
    {
        this.obstaclePrefab = obstaclePrefab;
    }

    private void CreateEnemyAbove(GameObject ground)
    {
        //var enemyCollider = enemyPrefab.GetComponent<Collider>();
        //float offsetY = 0;
        //if (enemyCollider != null)
        //    offsetY = -enemyCollider.bounds.min.y;

        if(enemyPrefab != null)
            Instantiate(enemyPrefab, ground.transform.position + new Vector3(0, ground.GetComponent<Renderer>().bounds.size.y, 0) /*+ Vector3.up * offsetY*/, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }

    private void CreateObstacleOn(Transform rail)
    {
        // get direction between rail blocks
        int clickedIndex = level.rail.FindIndex(t => t.position.x == rail.position.x && t.position.z == rail.position.z);
        Vector3 currentPosition = level.rail[clickedIndex].transform.position;
        Vector3 lastPosition = level.rail[clickedIndex - 1].transform.position;
        Quaternion platformDirection = Quaternion.LookRotation(currentPosition - lastPosition);
        
        // instantiate prefab
        if (obstaclePrefab != null)
            Instantiate(obstaclePrefab, rail.transform.position, platformDirection);
    }

}
