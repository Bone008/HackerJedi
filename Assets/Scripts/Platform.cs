using System;
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

    private float initialVelocity;
    private float startTime;
    private float journeyLength;

    private float blockedTime;

    void Start ()
    {
        initialVelocity = velocity;

        nextRailPart();
        transform.position = startPoint + platformOffset;
    }

    void Update()
    {
        // cheat
        if (GameData.Instance.cheatingMode && Input.GetKeyDown(KeyCode.F))
            velocity = 40;
        if (GameData.Instance.cheatingMode && Input.GetKeyUp(KeyCode.F))
            velocity = initialVelocity;

        blockedTime -= Time.deltaTime;

        // abort if stopped by a obstacle. add deltaTime to have correct Lerp after re-activating
        if (blockedTime > 0)
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
            else if (transform.position == endPoint + platformOffset && railPointer == level.rail.Count - 1)
            {
                FinishLevel();
            }
        }
        
    }

    void nextRailPart()
    {
        startTime = Time.time;
        startPoint = level.rail[railPointer].position;
        railPointer++;
        endPoint = level.rail[railPointer].position;
        journeyLength = Vector3.Distance(startPoint, endPoint);

        level.numPassedRails++;

        // close entrance gate when reaching 4th rail piece
        if(railPointer == 3)
            level.startMarker.GetComponent<EntranceExitRoomController>().CloseGate();
    }

    private void FinishLevel()
    {
        level.endMarker.GetComponent<EntranceExitRoomController>().CloseGate(() =>
        {
            if (GameData.Instance.currentLevel < GameData.Instance.levelCount)
            {
                SteamVR_LoadLevel.Begin("inbetween_game");
            }
            else
            {
                GameData.Instance.successfulHack = true;
                SteamVR_LoadLevel.Begin("end_of_game");
            }
        });

        this.enabled = false;
    }

    public Vector3 getVelocity()
    {
        if (blockedTime > 0)
            return Vector3.zero;
        else
            return (endPoint - startPoint).normalized * velocity;
    }

    public void DisableForSec(float seconds)
    {
        Debug.Assert(seconds > 0);
        blockedTime = seconds;
    }
    
}
