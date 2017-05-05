using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseText : MonoBehaviour {

    public Text text;
    public Text textH;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.None;
        if (GameData.Instance.successfulHack)
        {
            text.text = "the HackerJedi hacked the system with great force";
            text.color = new Color(255,0,0,255);
            textH.text = "You hacked the s#!t out of the Master !";
            textH.color = new Color(0, 255, 0, 255);
        }
        else
        {
            text.text = "Virus successfully moved to quarantine!";
            text.color = new Color(0, 255, 0, 255);
            textH.text = "a meaningfull message to the loser";
            textH.color = new Color(255, 0, 0, 255);
        }
	}

    public void backToMenu()
    {
        SteamVR_LoadLevel.Begin("StartMenu");
    }

}
