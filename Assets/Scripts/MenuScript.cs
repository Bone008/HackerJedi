using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public void startGame()
    {
        GameData.Instance.currentLevel = 1;
        SceneManager.LoadScene(1);
    }

    public void endGame()
    {
        Application.Quit();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
