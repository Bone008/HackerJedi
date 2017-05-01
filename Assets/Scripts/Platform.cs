using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour {
    
    private int railPointer = 0;
    public LevelGenerator level;

    private Vector3 startPoint;
    private Vector3 endPoint;
    public float velocity;

    private Vector3 platformOffset = Vector3.up * .6f;

    bool forward;

    private float startTime;
    private float journeyLength;

    [HideInInspector]
    public bool running = true;

    void Start ()
    {
        nextRailPart();
        transform.position = startPoint + platformOffset;

	}

    void Update()
    {
        // abort if stopped by a obstacle. add deltaTime to have correct Lerp after re-activating
        if (!running)
        {
            startTime += Time.deltaTime;
            return;
        }

        if(level.rail.Count != 0)
        {
            if (transform.position != level.rail[level.rail.Count - 1].position)
            {
                float distCovered = (Time.time - startTime) * velocity;
                float fracJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPoint + platformOffset, endPoint + platformOffset, fracJourney);
            }

            if (transform.position == endPoint + platformOffset && railPointer < level.rail.Count - 1)
            {
                nextRailPart();
            }
            else if (transform.position == endPoint + platformOffset && railPointer == level.rail.Count - 1 && GameData.Instance.levels != 0)
            {
                GameData.Instance.randomizeWorldinProgress = GameData.Instance.levels * (level.rows + level.lines);
                GameData.Instance.levels--;
                SceneManager.LoadScene(1);
            }
            else if (transform.position == endPoint + platformOffset && railPointer == level.rail.Count - 1 && GameData.Instance.levels == 0)
                SceneManager.LoadScene(2);
        }
        
    }

    void nextRailPart()
    {
        //if (railPointer > level.rail.Count - 2 && forward)
        //    forward = false;
        //else if (railPointer < 1 && !forward)
        //    forward = true;

        startTime = Time.time;
        if (/*forward*/ railPointer < level.rail.Count - 1)
        {
            startPoint = level.rail[railPointer].position;
            railPointer++;
            endPoint = level.rail[railPointer].position;
            level.numPassedRails++;
        }
        //else
        //{
        //    startPoint = level.rail[railPointer];
        //    railPointer--;
        //    endPoint = level.rail[railPointer];
        //}
        journeyLength = Vector3.Distance(startPoint, endPoint);
    }

    public Vector3 getVelocity()
    {
        return (endPoint - startPoint).normalized * velocity;
    }

    public void DisableForSec(float seconds)
    {
        StartCoroutine(TemporaryDisablePlatform(seconds));
    }

    private IEnumerator TemporaryDisablePlatform(float seconds)
    {
        running = false;
        yield return new WaitForSeconds(seconds);
        running = true;
    }
}
