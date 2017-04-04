using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public GameObject block;

    public int rows;
    public int lines;
    private GameObject[,] world;

    void createWorldPlane()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < lines; j++)
            {
                world[i, j] = Instantiate(block, new Vector3(i, 0, j), Quaternion.identity);
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
                    world[i, j].transform.position.Set(world[i, j].transform.position.x, Random.Range(0, 3), world[i, j].transform.position.z);

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
