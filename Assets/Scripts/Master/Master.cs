﻿using System;
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
    public LevelGenerator level;
    public GameObject noSpawnZone;
    public Material obstacleNoSpawnZoneMaterial;
    public Text spawnResourceText;
    public Text didntWorkMessage;
    private Coroutine didntWorkCoroutine;
    private SpawnResource spawnResource;
    private Dictionary<int, GameObject> placedObstacles = new Dictionary<int, GameObject>();
    private GameObject enemyPrefab = null;
    private GameObject obstaclePrefab = null;
    private Material defaultRailMaterial;

    [Header("Block Moving")]
    public float blockMinYValue = 0;
    public float blockMaxYValue = 6;
    public float blockSpeed = 30.0f;
    public Color liftedBlockColor;
    private Material defaultBlockMaterial;
    private Material liftedBlockMaterial;
    private bool currentlyDragging;
    private Vector2 oldMousePosition;

    [Header("Block Snapping To Grid")]
    public float snappingSpeed = 1.0f;
    public float snappingGround = 0.0f;
    public float snappingGridSize = 2.0f;
    private Dictionary<Transform, Coroutine> snapCoroutines = new Dictionary<Transform, Coroutine>();

    [Header("Laser Beam")]
    public VolumetricLines.VolumetricLineBehavior laserBeam;
    public Transform laserStart;
    private LookAtMouse lookAtMouseScript;

    [Header("Buttons")]
    #region
    public GameObject suicideRobot;
    public GameObject sniper;
    public GameObject turret;
    public GameObject hackingArea;
    public GameObject firewall;
    public GameObject fragmentation_Saw;
    #endregion

    private HashSet<Transform> selected = new HashSet<Transform>();

    void Start()
    {
        laserBeam.gameObject.SetActive(false);
        lookAtMouseScript = masterEye.GetComponent<LookAtMouse>();
        oldMousePosition = Input.mousePosition;
        platform = GameObject.FindWithTag("Platform").GetComponent<Platform>();

        // get default glow color of blocks from any block
        defaultBlockMaterial = level.world[0, 0].GetComponentInChildren<Renderer>().sharedMaterial;
        // create lifted material (so it can be shared by all lifted blocks)
        liftedBlockMaterial = new Material(defaultBlockMaterial);
        liftedBlockMaterial.SetColor("_MKGlowColor", liftedBlockColor);
        

        // get spawn resource
        spawnResource = GetComponent<SpawnResource>();
        spawnResource.onChange.AddListener(() => spawnResourceText.text = spawnResource.currentValue + " $");

        // get material of master rail
        defaultRailMaterial = level.rail[0].GetChild(1).GetComponent<Renderer>().material;
        
        // stop blinking of lifted blocks
        for (int i = 0; i < level.world.GetLength(0); i++)
        {
            for (int u = 0; u < level.world.GetLength(1); u++)
            {
                var block = level.world[i, u].transform;
                if (block.position.y > blockMinYValue && block.tag.Equals("RoomBlock"))
                    SetBlockMaterial(block, liftedBlockMaterial);
            }
        }

        // unlocked units and obstacles
        #region
        if (GameData.Instance.suicideRobotUnlocked && !suicideRobot.activeSelf)
            suicideRobot.SetActive(true);
        if (GameData.Instance.sniperUnlocked && !sniper.activeSelf)
            sniper.SetActive(true);
        if (GameData.Instance.turretUnlocked && !turret.activeSelf)
            turret.SetActive(true);
        if (GameData.Instance.hackingAreaUnlocked && !hackingArea.activeSelf)
            hackingArea.SetActive(true);
        if (GameData.Instance.firewallUnlocked && !firewall.activeSelf)
            firewall.SetActive(true);
        if (GameData.Instance.fragmentationSawUnlocked && !fragmentation_Saw.activeSelf)
            fragmentation_Saw.SetActive(true);
        #endregion

        // start no spawn zone
        StartCoroutine(AutoUpdateNoSpawnZone());
    }

    void Update()
    {
        // toggle cheating mode
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameData.Instance.cheatingMode = !GameData.Instance.cheatingMode;
            Debug.Log("Cheating mode enabled: " + GameData.Instance.cheatingMode);
        }

        // fill resource for testing purposes
        if (GameData.Instance.cheatingMode && Input.GetKeyDown(KeyCode.C)) // C for Cheat
        {
            spawnResource.ChangeValue(spawnResource.maxValue);
            suicideRobot.SetActive(true);
            sniper.SetActive(true);
            turret.SetActive(true);
            hackingArea.SetActive(true);
            firewall.SetActive(true);
            fragmentation_Saw.SetActive(true);
        }

        // select cube
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)))
        {
            // clear if not multidrag
            if (!(Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)))
                selected.Clear();

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // get aimed-for object via Raycast, prevent onclick when pressing button
                RaycastHit hit;
                Ray ray = masterCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayermask.value))
                {
                    selected.Add(hit.transform);
                }
            }
        }

        // spawn enemy
        if (selected.Count == 1 && Input.GetMouseButtonDown(0))
        {
            var s = selected.First();
            if (s.tag.Equals("RoomBlock"))
                CreateEnemyAbove(s.gameObject);
            else if (s.tag.Equals("RailBlock"))
                CreateObstacleOn(s);
        }

        // start dragging
        if (selected.Count > 0 && ((Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift)) || (Input.GetMouseButton(1) && Input.GetKeyUp(KeyCode.LeftShift))))
        {
            currentlyDragging = true;

            foreach (Transform s in selected.Where(t => t.tag.Equals("RoomBlock")))
            {
                SetBlockMaterial(s.parent, liftedBlockMaterial);

                if (snapCoroutines.ContainsKey(s.parent))
                {
                    Coroutine existing = snapCoroutines[s.parent];
                    if (existing != null)
                        StopCoroutine(existing);
                    snapCoroutines.Remove(s.parent);
                }
            }            
        }

        // stop dragging
        if (currentlyDragging && Input.GetMouseButtonUp(1))
        {
            currentlyDragging = false;

            foreach(Transform s in selected)
            {
                if (s == null || s.parent == null || !s.parent.tag.Equals("RoomBlock"))
                    return;

                if (snapCoroutines.ContainsKey(s.parent))
                {
                    Coroutine existing = snapCoroutines[s.parent];
                    if (existing != null)
                        StopCoroutine(existing);
                    snapCoroutines.Remove(s.parent);
                }

                snapCoroutines.Add(s.parent, StartCoroutine(SnapToGrid(s.parent)));
            }

            selected.Clear();
        }

        // move blocks
        Vector2 newMousePosition = Input.mousePosition;
        float mouseDragDiff = blockSpeed * (newMousePosition.y - oldMousePosition.y) * Time.deltaTime;
        if (selected.Count > 0 && currentlyDragging)
        {
            foreach (Transform s in selected)
            {
                if (!s.parent.tag.Equals("RoomBlock"))
                    continue;

                Transform parent = s.parent.transform;
                float y = parent.position.y + mouseDragDiff;
                y = Mathf.Max(blockMinYValue, y);
                y = Mathf.Min(blockMaxYValue, y);
                parent.position = new Vector3(
                    parent.position.x,
                    y,
                    parent.position.z
                );
            }
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
        if (selected.Count > 0 && currentlyDragging)
        {
            //laserBeam.SetPosition(0, laserStart.position);
            //laserBeam.SetPosition(1, selected.position);
            laserBeam.SetStartAndEndPoints(laserBeam.transform.InverseTransformPoint(laserStart.position), laserBeam.transform.InverseTransformPoint(selected.First().position));

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
            SetBlockMaterial(target, defaultBlockMaterial);
        
        snapCoroutines.Remove(target);
    }

    private void SetBlockMaterial(Transform block, Material m)
    {
        foreach (Renderer r in block.GetComponentsInChildren<Renderer>())
            r.sharedMaterial = m;
    }

    public void SelectEnemy(GameObject enemyPrefab)
    {
        this.enemyPrefab = enemyPrefab;
        UpdateNoSpawnZone();
    }

    public void SelectObstacle(GameObject obstaclePrefab)
    {
        this.obstaclePrefab = obstaclePrefab;
        UpdateNoSpawnZone();
    }

    private void CreateEnemyAbove(GameObject ground)
    {
        //var enemyCollider = enemyPrefab.GetComponent<Collider>();
        //float offsetY = 0;
        //if (enemyCollider != null)
        //    offsetY = -enemyCollider.bounds.min.y;

        if (enemyPrefab == null)
        {
            SpawnError("No enemy selected!");
            return;
        }

        // get enemy base script
        EnemyBase enemyBase = enemyPrefab.GetComponentInChildren<EnemyBase>();
        Debug.Assert(enemyBase != null);

        // check if too close to platform
        if (InNoSpawnZone(enemyBase, ground.transform.position))
        {
            SpawnError("Too close!");
            return;
        }

        // buy enemy
        float cost = enemyBase.placingCost;
        if (spawnResource.SafeChangeValue(-cost))
            Instantiate(enemyPrefab, ground.transform.position + new Vector3(0, ground.GetComponent<Renderer>().bounds.size.y, 0) /*+ Vector3.up * offsetY*/, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
        else
            SpawnError("Not enough money!");
    }

    private void CreateObstacleOn(Transform rail)
    {
        // index of the rail that was clicked
        int clickedIndex = level.rail.FindIndex(t => t.position.x == rail.position.x && t.position.z == rail.position.z);

        // dont put obstacles behind or inside the platform
        if (clickedIndex <= level.numPassedRails)
        {
            SpawnError("Cannot place behind platform!");
            return;
        }

        // dont put obstacles on already existing obstacles
        if (placedObstacles.ContainsKey(clickedIndex))
        {
            GameObject obstacle = placedObstacles[clickedIndex];
            if (obstacle == null)
                placedObstacles.Remove(clickedIndex); // stored obstacle has been destroyed
            else
            {
                SpawnError("Cannot place on obstacle!");
                return;
            }
        }

        // get direction between rail blocks
        Vector3 currentPosition = level.rail[clickedIndex].transform.position;
        Vector3 lastPosition = level.rail[clickedIndex - 1].transform.position;
        Quaternion platformDirection = Quaternion.LookRotation(currentPosition - lastPosition);

        if (obstaclePrefab == null)
        {
            SpawnError("No obstacle selected");
            return;
        }

        // get obstacle base
        ObstacleBase obstacleBase = obstaclePrefab.GetComponent<ObstacleBase>();
        Debug.Assert(obstacleBase != null);

        // check if too close to platform
        if ((clickedIndex - level.numPassedRails) < obstacleBase.minPlatformSpawnDist)
        {
            SpawnError("Too close!");
            return;
        }

        // buy obstacle
        float cost = obstacleBase.placingCost;
        if (spawnResource.SafeChangeValue(-cost))
        {
            GameObject placed = Instantiate(obstaclePrefab, rail.position, platformDirection);
            placedObstacles.Add(clickedIndex, placed);
        }
        else
        {
            SpawnError("Not enough money!");
        }
    }

    private bool InNoSpawnZone(EnemyBase enemyBase, Vector3 position)
    {
        Vector3 direction = platform.transform.position - position;
        direction.y = 0;
        return (direction.sqrMagnitude < enemyBase.minPlatformSpawnDist * enemyBase.minPlatformSpawnDist);
    }

    private IEnumerator AutoUpdateNoSpawnZone()
    {
        while (true)
        {
            UpdateNoSpawnZone();
            yield return new WaitForSeconds(1);
        }
    }

    private void SpawnError(string message)
    {
        if (didntWorkCoroutine != null)
            StopCoroutine(didntWorkCoroutine);

        didntWorkCoroutine = StartCoroutine(SpawnErrorCoroutine(message));
    }

    private IEnumerator SpawnErrorCoroutine(string message)
    {
        didntWorkMessage.text = message;
        didntWorkMessage.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        didntWorkMessage.transform.parent.gameObject.SetActive(false);
        didntWorkCoroutine = null;
    }

    private void UpdateNoSpawnZone()
    {
        // highlight blocks in no-spawn zone
        if (enemyPrefab != null)
        {
            EnemyBase enemyBase = enemyPrefab.GetComponentInChildren<EnemyBase>();
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

                Vector3 platformPos = platform.transform.position;
                // find adjacent corners
                Vector3? next = noSpawnCorners
                    .Where(v => (v - last).sqrMagnitude == bs * bs)
                    .OrderBy(v => (platformPos - v).sqrMagnitude)
                    .Select(v => new Vector3?(v))
                    .FirstOrDefault();

                if (!next.HasValue)
                {
                    // FIXME this should not happen
                    //Debug.Log("nope" + last);
                    continue;
                }
                if (!sorted.Exists(v => (next.Value - v).sqrMagnitude <= 0.1))
                    sorted.Add(next.Value);
                noSpawnCorners.Remove(next.Value);
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
                level.rail[i].GetChild(1).GetComponent<Renderer>().sharedMaterial = obstacleNoSpawnZoneMaterial;

            for (int i = level.numPassedRails + ob.minPlatformSpawnDist; i < level.rail.Count; i++)
                level.rail[i].GetChild(1).GetComponent<Renderer>().sharedMaterial = defaultRailMaterial;
        }
    }

}
