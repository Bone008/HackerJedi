using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseText : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
        if (GameData.Instance.successfulHack)
        {
            Cursor.lockState = CursorLockMode.None;
            text.text = "the HackerJedi hacked the system with great force";
            text.color = new Color(255,0,0,255);
        }
        else
        {
            text.text = "Virus successfully moved to quarantine!";
            text.color = new Color(0, 255, 0, 255);
        }
	}

    public void backToMenu()
    {
        SteamVR_LoadLevel.Begin("StartMenu");
    }

}
