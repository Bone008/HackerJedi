 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject block;
    public GameObject railBlock;
    public GameObject startMarker;
    public GameObject endMarker;

    public float blockSize;

    [HideInInspector]
    public List<Transform> rail;
    public int numPassedRails { get; set; }

    // 1:randomPresetBlock Wahrscheinlichkeit dass ein Block versetzt wird; 0 keine Blöcke werden versetzt
    public int randomPresetBlock;
    // trackRowOffset: freie blöcke neben dem track, minimal = 0 - maximal = row / 2
    public int trackRowOffset;

    public int rows;
    public int lines;
    public GameObject[,] world;
    public GameObject[,] track;
    
    public void clearWorld()
    {
        rail.Clear();
        startMarker.SetActive(false);
        endMarker.SetActive(false);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                Destroy(world[i, j]);
            }
        }
    }

    void fillWorldPlane()
    {
        Vector3 centerOffset = new Vector3((rows - 1) / 2f, 0, (lines - 1) / 2f) * blockSize;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                if(world[i, j] == null)
                {
                    world[i, j] = Instantiate(block, new Vector3(i * blockSize, 0, j * blockSize) - centerOffset, Quaternion.identity);
                    world[i, j].transform.SetParent(transform, false);
                    world[i, j].tag = "RoomBlock";
                    foreach (Transform t in world[i, j].transform)
                        t.tag = "RoomBlock";
                }
            }
        }
    }

    void randomizeWorld(int r)
    {
        if (r == 0)
            return;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                if (Random.Range(0, r) == 0 && world[i, j].tag.Equals("RoomBlock"))
                {
                    if (world[i, j].transform.position.y == 0)
                    {
                        #region Null_Block
                        if (Random.Range(0, 2) == 1)
                        {
                            world[i, j].transform.Translate(Vector3.up * 2 * blockSize);
                        }
                        else
                        {
                            world[i, j].transform.Translate(Vector3.up * blockSize);
                        }
                        #endregion
                    }
                    else if (world[i, j].transform.position.y == 2 * blockSize)
                    {
                        #region Top_Block
                        if (Random.Range(0, 2) == 1)
                        {
                            world[i, j].transform.Translate(Vector3.down * 2 * blockSize);
                        }
                        else
                        {
                            world[i, j].transform.Translate(Vector3.down * blockSize);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Mid_Block
                        if (Random.Range(0, 2) == 1)
                        {
                            world[i, j].transform.Translate(Vector3.up * blockSize);
                        }
                        else
                        {
                            world[i, j].transform.Translate(Vector3.down * blockSize);
                        }
                        #endregion
                    }
                }
            }
        }
    }

    public void createTrack()
    {
        int initialP = Random.Range(trackRowOffset, rows - trackRowOffset);
        int p = initialP;
        int direction = 0;
        int prevDirection;
        int sideway;

        for (int i = 0; i < lines; i++)
        {
            prevDirection = direction;
            Vector3 centerOffset = new Vector3((rows - 1) / 2f, 0, (lines - 1) / 2f) * blockSize;

            world[p, i] = Instantiate(railBlock, new Vector3(p * blockSize, 0, i * blockSize) - centerOffset + (Vector3.up), Quaternion.identity);
            world[p, i].transform.SetParent(transform, false);
            world[p, i].tag = "RailBlock";
            rail.Add(world[p, i].transform);

            direction = Random.Range(0, 3);

            if (direction == 1 && prevDirection != 2 && p > trackRowOffset && i < lines - 3 && i > 0)
            {
                sideway = Random.Range(2, p - trackRowOffset);

                for(int j = 0; j < sideway; j++)
                {
                    world[p, i + 1] = Instantiate(railBlock, new Vector3(p * blockSize, 0, (i + 1) * blockSize) - centerOffset + (Vector3.up), Quaternion.identity);
                    world[p, i + 1].transform.SetParent(transform, false);
                    world[p, i + 1].tag = "RailBlock";
                    rail.Add(world[p, i + 1].transform);
                    p--;
                }
            }
            else if (direction == 2 && prevDirection != 1 && p < (rows - (1 + trackRowOffset)) && i < lines - 3 && i > 0)
            {
                sideway = Random.Range(2, rows - (p + (1 + trackRowOffset)));

                for(int j = 0; j < sideway; j++)
                {
                    world[p, i + 1] = Instantiate(railBlock, new Vector3(p * blockSize, 0, (i + 1) * blockSize) - centerOffset + (Vector3.up), Quaternion.identity);
                    world[p, i + 1].transform.SetParent(transform, false);
                    world[p, i + 1].tag = "RailBlock";
                    rail.Add(world[p, i + 1].transform);
                    p++;
                }
            }
        }

        startMarker.transform.position = world[initialP, 0].transform.position;
        startMarker.SetActive(true);
        endMarker.transform.position = world[p, lines - 1].transform.position;
        endMarker.SetActive(true);

        fillWorldPlane();
    }

    private void Awake()
    {
        world = new GameObject[rows, lines];
        track = new GameObject[rows, lines];
        numPassedRails = 0;
        createTrack();
        randomizeWorld(GameData.Instance.randomizeWorldFactor);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            clearWorld();
        }
        if (Input.GetKeyUp("l"))
            createTrack();
    }
}
