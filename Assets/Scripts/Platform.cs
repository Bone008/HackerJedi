using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    private List<Transform> rail;
    private int railLength;
    private int railPointer = 0;
    public LevelGenerator level;

    private Transform startPoint;
    private Transform endPoint;
    public float velocity;

    private Vector3 platformOffset = Vector3.up * 1.5f /*+ (Vector3.right * .75f) + (Vector3.forward * .75f)*/;

    bool forward;

    private float startTime;
    private float journeyLength;

    void Start ()
    {
        setupRail();
        nextRailPart();
        transform.position = startPoint.position + platformOffset;

	}

    void Update()
    {
        float distCovered = (Time.time - startTime) * velocity;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPoint.position + platformOffset, endPoint.position + platformOffset, fracJourney);
 
        if (transform.position == endPoint.position + platformOffset)
        {
            nextRailPart();
        }
    }

    void nextRailPart()
    {
        if (railPointer > railLength - 2 && forward)
            forward = false;
        else if (railPointer < 1 && !forward)
            forward = true;

        startTime = Time.time;
        if (forward)
        {
            startPoint = rail[railPointer];
            railPointer++;
            endPoint = rail[railPointer];
        }
        else
        {
            startPoint = rail[railPointer];
            railPointer--;
            endPoint = rail[railPointer];
        }
        journeyLength = Vector3.Distance(startPoint.position, endPoint.position);
    }

    void setupRail()
    {
        rail = level.rail;
        railLength = level.railLength;
    }

    public Vector3 getVelocity()
    {
        return (endPoint.position - startPoint.position).normalized * velocity;
    }

}
