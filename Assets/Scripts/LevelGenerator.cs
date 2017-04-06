using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject block;
    public GameObject railBlock;
    public GameObject endMarker;

    public float blockSize;

    public List<Transform> rail;

    // 1:randomPresetBlock Wahrscheinlichkeit dass ein Block versetzt wird; 0 keine Blöcke werden versetzt
    public int randomPresetBlock;

    public int rows;
    public int lines;
    private GameObject[,] world;
    public GameObject[,] track;

    void createWorldPlane()
    {
        Vector3 centerOffset = new Vector3((rows - 1) / 2f, 0, (lines - 1) / 2f) * blockSize;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                track[i, j] = Instantiate(railBlock, new Vector3(i * blockSize, 0, j * blockSize) - centerOffset + (Vector3.down * 0.5f), Quaternion.identity);
                track[i, j].transform.SetParent(transform, false);
                track[i, j].tag = "RailBlock";
                track[i, j].SetActive(false);

                world[i, j] = Instantiate(block, new Vector3(i * blockSize, 0, j * blockSize) - centerOffset, Quaternion.identity);
                world[i, j].transform.SetParent(transform, false);
                foreach (Transform t in world[i, j].transform)
                {
                    t.gameObject.tag = "RoomBlock";
                }
            }
        }
    }

    void flattenWorld()
    {
        rail.Clear();
        endMarker.SetActive(false);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                if (world[i, j].transform.position.y == 2 * blockSize)
                {
                    world[i, j].transform.Translate(Vector3.down * 2 * blockSize);
                }
                else if (world[i, j].transform.position.y == 1 * blockSize)
                {
                    world[i, j].transform.Translate(Vector3.down * blockSize);
                }
                else if (!world[i, j].activeSelf)
                {
                    track[i, j].SetActive(false);
                    world[i, j].SetActive(true);
                }
            }
        }
    }

    void randomizeWorld(int r)
    {
        createTrack();

        if (r == 0)
            return;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                if (Random.Range(0, r) == 0 && !track[i, j].activeSelf)
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

    void createTrack()
    {
        int p = Random.Range((rows / 4), (rows / 4) * 3);
        int direction = 0;
        int prevDirection;

        for (int i = 0; i < lines; i++)
        {
            prevDirection = direction;
            rail.Add(world[p, i].transform);
            world[p, i].SetActive(false);
            track[p, i].SetActive(true);
            //world[p+1, i].SetActive(false);
            //track[p+1, i].SetActive(true);
            direction = Random.Range(0, 3);
            if (direction == 1 && prevDirection != 2 && p > 1 && i < lines - 2)
            {
                rail.Add(world[p, i + 1].transform);
                world[p, i + 1].SetActive(false);
                track[p, i + 1].SetActive(true);
                //world[p+1, i + 1].SetActive(false);
                //track[p+1, i + 1].SetActive(true);
                p--;
            }
            else if (direction == 2 && prevDirection != 1 && p < (rows - 2) && i < lines - 2)
            {
                rail.Add(world[p, i + 1].transform);
                world[p, i + 1].SetActive(false);
                track[p, i + 1].SetActive(true);
                //world[p+1, i + 1].SetActive(false);
                //track[p+1, i + 1].SetActive(true);
                p++;
            }
        }
        endMarker.transform.position = track[p, lines - 1].transform.position;
        endMarker.SetActive(true);
    }

    private void Awake()
    {
        world = new GameObject[rows, lines];
        track = new GameObject[rows, lines];
        createWorldPlane();
        randomizeWorld(randomPresetBlock);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            flattenWorld();
        }
        if (Input.GetKeyUp("space"))
            randomizeWorld(randomPresetBlock);
    }
}
