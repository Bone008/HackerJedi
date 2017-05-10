using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseText : MonoBehaviour {

    public Text text;
    public Text textH;
    public GameObject bluescreen;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.None;
        if (GameData.Instance.successfulHack)
        {
            text.text = ""; // covered by bluescreen
            text.color = new Color(255,0,0,255);
            textH.text = "Access granted\nYou eliminated Rogue A.I. and saved the world.";
            textH.color = new Color(0, 255, 0, 255);
            bluescreen.SetActive(true);
        }
        else
        {
            text.text = "The  intruder  was  successfully  moved  to  quarantine.";
            text.color = new Color(0, 255, 0, 255);
            textH.text = "Access  denied\nYour  hacking  attempt  failed.";
            textH.color = new Color(255, 0, 0, 255);
            bluescreen.SetActive(false);
        }
	}

    public void backToMenu()
    {
        SteamVR_LoadLevel.Begin("StartMenu");
    }

}
