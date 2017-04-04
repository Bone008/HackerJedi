using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public GameObject block;
    public float blockSize;

    public int rows;
    public int lines;
    private GameObject[,] world;

    void createWorldPlane()
    {
        Vector3 centerOffset = new Vector3((rows-1) / 2f, 0, (lines-1) / 2f) * blockSize;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                world[i, j] = Instantiate(block, new Vector3(i * blockSize, 0, j * blockSize) - centerOffset, Quaternion.identity);
                world[i, j].transform.SetParent(transform, false);
            }
        }
    }

    void randomizeWorld()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                Debug.Log(i + ":" + j);
                if (Random.Range(0, 10) == 5)
                    world[i, j].transform.localPosition += new Vector3(0, Random.Range(0, 3), 0);

            }
        }
    }

    // Use this for initialization
    void Start()
    {
        world = new GameObject[rows, lines];
        createWorldPlane();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
