using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour {

    private List<Transform> rail;
    private int railPointer = 0;
    public LevelGenerator level;

    private Transform startPoint;
    private Transform endPoint;
    public float velocity;

    private Vector3 platformOffset = Vector3.up * 1.6f /*+ (Vector3.right * .75f) + (Vector3.forward * .75f)*/;

    bool forward;

    private float startTime;
    private float journeyLength;

    [HideInInspector]
    public bool running = true;

    void Start ()
    {
        setupRail();
        nextRailPart();
        transform.position = startPoint.position + platformOffset;

	}

    void Update()
    {
        // abort if stopped by a obstacle. add deltaTime to have correct Lerp after re-activating
        if (!running)
        {
            startTime += Time.deltaTime;
            return;
        }

        float distCovered = (Time.time - startTime) * velocity;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPoint.position + platformOffset, endPoint.position + platformOffset, fracJourney);

        if (transform.position == endPoint.position + platformOffset && railPointer != rail.Count - 1)
        {
            nextRailPart();
        } else

        if (transform.position == endPoint.position + platformOffset && railPointer == rail.Count - 1)
        {
            SceneManager.LoadScene(1);
        }
        
    }

    void nextRailPart()
    {
        if (railPointer > rail.Count - 2 && forward)
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
    }

    public Vector3 getVelocity()
    {
        return (endPoint.position - startPoint.position).normalized * velocity;
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
