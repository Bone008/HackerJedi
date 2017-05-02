using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SpawnResource))]
public class Master : MonoBehaviour {

    public Camera masterCamera;
    public Transform masterEye;
    public LayerMask raycastLayermask;
    private Platform platform;

    [Header("Movement")]
    //public Transform movementMin;
    //public Transform movementMax;
    public float maxMovementDelta;
    public string movementInputAxis;

    [Header("Spawning")]
    private GameObject enemyPrefab = null;
    private GameObject obstaclePrefab = null;
    public LevelGenerator level;
    private Dictionary<int, GameObject> placedObstacles = new Dictionary<int, GameObject>();
    private SpawnResource spawnResource;
    public GameObject noSpawnZone;
    public Material obstacleNoSpawnZoneMaterial;
    private Material defaultRailMaterial;

    [Header("Block Moving")]
    public float blockMinYValue = 0;
    public float blockMaxYValue = 6;
    public float blockSpeed = 30.0f;
    private bool currentlyDragging;
    private Vector2 oldMousePosition;
    private Color defaultBlockColor;
    public Color liftedBlockColor;

    [Header("Block Snapping To Grid")]
    public float snappingSpeed = 1.0f;
    public float snappingGround = 0.0f;
    public float snappingGridSize = 2.0f;
    private Dictionary<Transform, Coroutine> snapCoroutines = new Dictionary<Transform, Coroutine>();

    [Header("Laser Beam")]
    public VolumetricLines.VolumetricLineBehavior laserBeam;
    public Transform laserStart;
    private LookAtMouse lookAtMouseScript;

    private Transform selected;

    void Start()
    {
        laserBeam.gameObject.SetActive(false);
        lookAtMouseScript = masterEye.GetComponent<LookAtMouse>();
        oldMousePosition = Input.mousePosition;
        platform = GameObject.FindWithTag("Platform").GetComponent<Platform>();

        // get default glow color of blocks from any block
        defaultBlockColor = level.world[0, 0].GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor");

        // get spawn resource
        spawnResource = GetComponent<SpawnResource>();

        defaultRailMaterial = level.rail[0].GetComponent<Renderer>().material;
    }
    
    void Update()
    {
        // fill resource for testing purposes
        // TODO remove
        if (Input.GetKeyDown(KeyCode.C)) // C for Cheat
            spawnResource.ChangeValue(spawnResource.maxValue);

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
        if (Input.GetMouseButtonDown(1) && selected != null && selected.tag.Equals("RoomBlock"))
        {
            SetBlockColor(selected.parent, liftedBlockColor);

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
        if (currentlyDragging && Input.GetMouseButtonUp(1))
        {
            currentlyDragging = false;

            if (selected == null || selected.parent == null)
                return;

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
            transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), -input * maxMovementDelta * Time.deltaTime);
        }

        // move laser
        if (selected != null && currentlyDragging)
        {
            //laserBeam.SetPosition(0, laserStart.position);
            //laserBeam.SetPosition(1, selected.position);
            laserBeam.SetStartAndEndPoints(laserBeam.transform.InverseTransformPoint(laserStart.position), laserBeam.transform.InverseTransformPoint(selected.position));

            if(!laserBeam.gameObject.activeSelf)
            {
                laserBeam.gameObject.SetActive(true);
                lookAtMouseScript.rotationSpeed *= 10.0f;
            }            
        }
        else
        {
            if(laserBeam.gameObject.activeSelf)
            {
                laserBeam.gameObject.SetActive(false);
                lookAtMouseScript.rotationSpeed /= 10.0f;
            }            
        }

        // highlight blocks in no-spawn zone
        if (enemyPrefab != null)
        {
            EnemyBase enemyBase = enemyPrefab.GetComponent<EnemyBase>();
            float bs = level.blockSize;
            float hs = level.blockSize / 2;
            float targetY = hs + 0.2f;

            // find blocks inside the no spawn zone and add its corners to the list
            List<Vector3> noSpawnBlocks = new List<Vector3>(); // TODO maybe replace with set
            for (int i = 0; i < level.world.GetLength(0); i++)
            {
                for (int u = 0; u < level.world.GetLength(1); u++)
                {
                    Transform block = level.world[i, u].transform;
                    if (InNoSpawnZone(enemyBase, block.position))
                    {
                        noSpawnBlocks.Add(block.position);
                    }
                }
            }

            // filter for blocks that are in the middle and add corners
            List<Vector3> noSpawnCorners = new List<Vector3>();
            Vector3 minLevel = level.world[0, 0].transform.position;
            Vector3 maxLevel = level.world[level.rows - 1, level.lines - 1].transform.position;

            for (int i = 0; i < noSpawnBlocks.Count; i++)
            {
                Vector3 pos = noSpawnBlocks[i];

                // get surrounding blocks
                bool leftInZone = pos.x > minLevel.x && InNoSpawnZone(enemyBase, pos + new Vector3(-bs, 0, 0));
                bool rightInZone = pos.x < maxLevel.x && InNoSpawnZone(enemyBase, pos + new Vector3(+bs, 0, 0));
                bool upInZone = pos.z < maxLevel.z && InNoSpawnZone(enemyBase, pos + new Vector3(0, 0, +bs));
                bool downInZone = pos.z > minLevel.z && InNoSpawnZone(enemyBase, pos + new Vector3(0, 0, -bs));

                // exclude blocks in the middle
                if (leftInZone && rightInZone && upInZone && downInZone)
                    continue;

                // add outer corners
                if (!leftInZone)
                {
                    noSpawnCorners.Add(pos + new Vector3(-hs, -pos.y + targetY, +hs));
                    noSpawnCorners.Add(pos + new Vector3(-hs, -pos.y + targetY, -hs));
                }
                if (!rightInZone)
                {
                    noSpawnCorners.Add(pos + new Vector3(+hs, -pos.y + targetY, +hs));
                    noSpawnCorners.Add(pos + new Vector3(+hs, -pos.y + targetY, -hs));
                }
                if (!upInZone)
                {
                    noSpawnCorners.Add(pos + new Vector3(+hs, -pos.y + targetY, +hs));
                    noSpawnCorners.Add(pos + new Vector3(-hs, -pos.y + targetY, +hs));
                }
                if (!downInZone)
                {
                    noSpawnCorners.Add(pos + new Vector3(+hs, -pos.y + targetY, -hs));
                    noSpawnCorners.Add(pos + new Vector3(-hs, -pos.y + targetY, -hs));
                }
            }

            noSpawnCorners = noSpawnCorners.Distinct().ToList();

            // sort them to connect only adjacent corners
            List<Vector3> sorted = new List<Vector3>();
            sorted.Add(noSpawnCorners[0]);
            noSpawnCorners.RemoveAt(0);
            int initialCount = noSpawnCorners.Count;
            for (int i = 0; i < initialCount; i++)
            {
                Vector3 last = sorted.Last();

                // find adjacent corners
                Vector3[] nexts = noSpawnCorners
                    .FindAll(v => (v - last).sqrMagnitude == bs * bs)
                    .OrderBy(v => v, new VecComparer() { platform = this.platform.transform.position })
                    .ToArray();

                if (nexts.Length == 0)
                {
                    // FIXME this should not happen
                    //Debug.Log("nope" + last);
                    continue;
                }

                Vector3 next = nexts[0];
                sorted.Add(next);
                noSpawnCorners.Remove(next);
            }
            
            // finish circle
            sorted.Add(sorted[0]);

            // pass corners to line renderer
            Vector3[] vertices = sorted.ToArray();            
            var lr = noSpawnZone.GetComponent<LineRenderer>();
            lr.positionCount = vertices.Length;
            lr.SetPositions(vertices);
        }

        // highlight no spawn zone of obstacles
        if (obstaclePrefab != null)
        {
            ObstacleBase ob = obstaclePrefab.GetComponentInChildren<ObstacleBase>();
            for (int i = 0; i < level.numPassedRails + ob.minPlatformSpawnDist; i++)
                level.rail[i].GetComponent<Renderer>().material = obstacleNoSpawnZoneMaterial;

            for (int i = level.numPassedRails + ob.minPlatformSpawnDist; i < level.rail.Count; i++)
                level.rail[i].GetComponent<Renderer>().material = defaultRailMaterial;
        }
    }

    private class VecComparer : IComparer<Vector3>
    {
        public Vector3 platform;

        public int Compare(Vector3 y, Vector3 x)
        {
            return (int)((platform - y).sqrMagnitude - (platform - x).sqrMagnitude);
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

        if (targetY == blockMinYValue)
            SetBlockColor(target, defaultBlockColor);
        
        snapCoroutines.Remove(target);
    }

    private void SetBlockColor(Transform block, Color c)
    {
        foreach(Renderer r in block.GetComponentsInChildren<Renderer>())
            r.material.SetColor("_MKGlowColor", c);
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

        if (enemyPrefab == null)
            return;

        // get enemy base script
        EnemyBase enemyBase = enemyPrefab.GetComponent<EnemyBase>();
        Debug.Assert(enemyBase != null);

        // check if too close to platform
        if (InNoSpawnZone(enemyBase, ground.transform.position))
            return;

        // buy enemy
        float cost = enemyBase.placingCost;
        if (spawnResource.SafeChangeValue(-cost))
            Instantiate(enemyPrefab, ground.transform.position + new Vector3(0, ground.GetComponent<Renderer>().bounds.size.y, 0) /*+ Vector3.up * offsetY*/, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
    }

    private void CreateObstacleOn(Transform rail)
    {
        // index of the rail that was clicked
        int clickedIndex = level.rail.FindIndex(t => t.position.x == rail.position.x && t.position.z == rail.position.z);

        // dont put obstacles behind or inside the platform
        if (clickedIndex <= level.numPassedRails)
            return;

        // dont put obstacles on already existing obstacles
        if (placedObstacles.ContainsKey(clickedIndex))
        {
            GameObject obstacle = placedObstacles[clickedIndex];
            if (obstacle == null)
                placedObstacles.Remove(clickedIndex); // stored obstacle has been destroyed
            else
                return;
        }

        // get direction between rail blocks
        Vector3 currentPosition = level.rail[clickedIndex].transform.position;
        Vector3 lastPosition = level.rail[clickedIndex - 1].transform.position;
        Quaternion platformDirection = Quaternion.LookRotation(currentPosition - lastPosition);

        if (obstaclePrefab == null)
            return;

        // get obstacle base
        ObstacleBase obstacleBase = obstaclePrefab.GetComponent<ObstacleBase>();
        Debug.Assert(obstacleBase != null);

        // check if too close to platform
        if ((clickedIndex - level.numPassedRails) < obstacleBase.minPlatformSpawnDist)
            return;

        // buy obstacle
        float cost = obstacleBase.placingCost;
        if (spawnResource.SafeChangeValue(-cost))
        {
            GameObject placed = Instantiate(obstaclePrefab, rail.position, platformDirection);
            placedObstacles.Add(clickedIndex, placed);
        }
    }

    private bool InNoSpawnZone(EnemyBase enemyBase, Vector3 position)
    {
        return ((platform.transform.position - position).sqrMagnitude < enemyBase.minPlatformSpawnDist * enemyBase.minPlatformSpawnDist);
    }

}
