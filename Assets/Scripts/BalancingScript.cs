using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancingScript : MonoBehaviour {

    GameObject hacker;
    GameObject master;

	// Use this for initialization
	void Awake() {
        hacker = GameObject.FindGameObjectWithTag("Player");
        master = GameObject.FindGameObjectWithTag("Master");

        if(GameData.Instance.hackerDiff == 0)
        {
            hacker.GetComponent<HealthResource>().maxValue = 1000;
            //Debug.Log("Hacker: Easy");
        }
        else if(GameData.Instance.hackerDiff == 1)
        {
            hacker.GetComponent<HealthResource>().maxValue = 500;
            //Debug.Log("Hacker: Normal");
        }
        else if (GameData.Instance.hackerDiff == 2)
        {
            hacker.GetComponent<HealthResource>().maxValue = 300;
            //Debug.Log("Hacker: Hard");
        }


        if (GameData.Instance.masterDiff ==  0)
        {
            master.GetComponent<Regenerative>().intervalS = .5f;
            //Debug.Log("Master: Easy");
        }
        else if (GameData.Instance.masterDiff == 1)
        {
            master.GetComponent<Regenerative>().intervalS = 1;
            //Debug.Log("Master: Normal");
        }
        else if (GameData.Instance.masterDiff == 2)
        {
            master.GetComponent<Regenerative>().intervalS = 2;
            //Debug.Log("Master: Hard");
        }
    }
}
