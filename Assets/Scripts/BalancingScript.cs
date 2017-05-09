using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancingScript : MonoBehaviour {

    GameObject hacker;
    GameObject master;

	// Use this for initialization
	void Start () {
        hacker = GameObject.FindGameObjectWithTag("Player");
        master = GameObject.FindGameObjectWithTag("MasterEye");

        if(GameData.Instance.hackerDiff == 0) //easy
            hacker.GetComponent<HealthResource>().maxValue = 1000;
        else if(GameData.Instance.hackerDiff == 2) //hard
            hacker.GetComponent<HealthResource>().maxValue = 300;
        else //normal
            hacker.GetComponent<HealthResource>().maxValue = 500;


        if(GameData.Instance.masterDiff == 0) //easy
            master.GetComponent<Regenerative>().intervalS = .5f;
        else if(GameData.Instance.masterDiff == 2) // hard
            master.GetComponent<Regenerative>().intervalS = 2;
        else //normal
            master.GetComponent<Regenerative>().intervalS = 1;
    }
}
