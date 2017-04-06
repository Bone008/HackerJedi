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
    private Dictionary<Transform, Coroutine> snapCoroutines = new Dictionary<Transform, Coroutine>();

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

            if(snapCoroutines.ContainsKey(selected.parent))
            {
                Coroutine existing = snapCoroutines[selected.parent];
                if(existing != null)
                    StopCoroutine(existing);
                snapCoroutines.Remove(selected.parent);
            }
        }

        // stop dragging
        if (Input.GetMouseButtonUp(1))
        {
            currentlyDragging = false;

            if (snapCoroutines.ContainsKey(selected.parent))
            {
                Coroutine existing = snapCoroutines[selected.parent];
                if (existing != null)
                    StopCoroutine(existing);
                snapCoroutines.Remove(selected.parent);
            }

            snapCoroutines.Add(selected.parent, StartCoroutine(SnapToGrid(selected.parent)));
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
        
        // move master
        float input = Input.GetAxis(movementInputAxis);
        if (input != 0)
        {
            // rotate on axis between two targets
            //Transform target = input < 0 ? movementMin : movementMax;
            //transform.position = Vector3.MoveTowards(transform.position, target.position, maxMovementDelta);

            // rotate around the center of the map
            transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), -input);
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

    private IEnumerator SnapToGrid(Transform box)
    {
        Transform target = box;

        // determine where to move to clip to grid
        float currentY = target.transform.position.y;
        int currentGridIndex = Mathf.FloorToInt((currentY - snappingGround) / snappingGridSize);
        float midY = snappingGround + (currentGridIndex * snappingGridSize) + (snappingGridSize / 2.0f);

        float targetY;
        if (currentY < midY)
            targetY = snappingGround + (currentGridIndex * snappingGridSize);
        else
            targetY = snappingGround + ((currentGridIndex + 1) * snappingGridSize);

        while (target.transform.position.y != targetY)
        {
            Vector3 pos = target.transform.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, Time.deltaTime * snappingSpeed);
            target.transform.position = pos;
            yield return null;
        }

        snapCoroutines.Remove(target);
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
