using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public Slider m;
    public Slider h;

    public Text mT;
    public Text hT;

    public InputField i;
    public Toggle t;
    public Toggle tW;

    // Use this for initialization
    void Start () {
        HackerProgression.Instance.Reset();
        masterSkillsAllOnOff(false);
        setHackerDiff();
        setMasterDiff();
        GameData.Instance.levelCount = 3;
        fullEquip();
        randWorld();
	}

    void masterSkillsAllOnOff(bool b)
    {
        GameData.Instance.firewallUnlocked = b;
        GameData.Instance.hackingAreaUnlocked = b;
        GameData.Instance.sniperUnlocked = b;
        GameData.Instance.suicideRobotUnlocked = b;
        GameData.Instance.turretUnlocked = b;
    }

    public void randWorld()
    {
        if (tW.isOn)
        {
            GameData.Instance.randomizeWorldFactor = 0;
            //Debug.Log("Flat");
        }
        else
        {
            GameData.Instance.randomizeWorldFactor = 8;
            //Debug.Log("Random");
        }
    }

    public void fullEquip()
    {
        if (t.isOn)
        {
            HackerProgression.Instance.UnlockAll();
            Debug.Log("Unlock");
        }
        else
        {
            HackerProgression.Instance.Reset();
            Debug.Log("Lock");
        }

        masterSkillsAllOnOff(t.isOn);
    }

    public void setLevelNumber()
    {
        int x;
        int.TryParse(i.text, out x);
        if (x <= 1)
        {
            i.text = ""+1;
            GameData.Instance.levelCount = 1;
        }
        else
            GameData.Instance.levelCount = x;
        Debug.Log(GameData.Instance.levelCount);
    }

    public void setMasterDiff()
    {
        GameData.Instance.masterDiff = m.value;

        if(GameData.Instance.masterDiff == 0)
        {
            mT.text = "Master Mode:\nEasy";
        }
        else if(GameData.Instance.masterDiff == 1)
        {
            mT.text = "Master Mode:\nNormal";
        }
        else if(GameData.Instance.masterDiff == 2)
        {
            mT.text = "Master Mode:\nHard";
        }
    }

    public void setHackerDiff()
    {
        GameData.Instance.hackerDiff = h.value;

        if (GameData.Instance.hackerDiff == 0)
        {
            hT.text = "Hacker Mode:\nEasy";
        }
        else if (GameData.Instance.hackerDiff == 1)
        {
            hT.text = "Hacker Mode:\nNormal";
        }
        else if (GameData.Instance.hackerDiff == 2)
        {
            hT.text = "Hacker Mode:\nHard";
        }
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
