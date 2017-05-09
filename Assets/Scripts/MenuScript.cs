using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        HackerProgression.Instance.Reset();
	}
	
    public void startGame()
    {
        GameData.Instance.currentLevel = 1;
        if(!SteamVR_LoadLevel.loading)
            SteamVR_LoadLevel.Begin("game");
    }

    public void startUnlocked()
    {
        GameData.Instance.UnlockEverything();
        HackerProgression.Instance.UnlockEverything();
        startGame();
    }

    public void endGame()
    {
        Application.Quit();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
