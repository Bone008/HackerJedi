using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour {

    [Header("Ready texts")]
    public Text readyHackerText;
    public Text readyMasterText;

    [Header("Buttons")]
    public Button gruntUpgrade;
    public Button suicideRobot;
    public Button sniper;
    public Button turret;
    public Button hackingArea;
    public Button firewall;
    public Button fragmentationSaw;

    public Text levelText;
    public Text pointsText;

    private int points;
    
    void Start()
    {
        points = 2;

        GameData.Instance.hackerIsReady = GameData.Instance.masterIsReady = false;

        levelText.text = string.Format("Hacker has completed stage {0}!", GameData.Instance.currentLevel);

        if (GameData.Instance.suicideRobotUnlocked)
            suicideRobot.interactable = false;
        if (GameData.Instance.sniperUnlocked)
            sniper.interactable = false;
        if (GameData.Instance.turretUnlocked)
            turret.interactable = false;
        if (GameData.Instance.hackingAreaUnlocked)
            hackingArea.interactable = false;
        if (GameData.Instance.firewallUnlocked)
            firewall.interactable = false;
        if (GameData.Instance.fragmentationSawUnlocked)
            fragmentationSaw.interactable = false;
    }

    public void hackerToggleReady()
    {
        GameData.Instance.hackerIsReady = !GameData.Instance.hackerIsReady;
        Debug.Log("Upgrade: hacker is ready: " + GameData.Instance.hackerIsReady);
        UpdateReadyTexts();
    }

    public void masterToggleReady()
    {
        GameData.Instance.masterIsReady = !GameData.Instance.masterIsReady;
        Debug.Log("Upgrade: master is ready: " + GameData.Instance.masterIsReady);
        UpdateReadyTexts();
    }

    private void UpdateReadyTexts()
    {
        bool h = GameData.Instance.hackerIsReady;
        bool m = GameData.Instance.masterIsReady;
        readyHackerText.text = (h && m ? "Let's go!" : (h ? "wait for master" : "Continue"));
        readyMasterText.text = (h && m ? "Let's go!" : (m ? "wait for hacker" : "Continue"));
    }

    public void Update()
    {
        if (GameData.Instance.hackerIsReady && GameData.Instance.masterIsReady && !SteamVR_LoadLevel.loading)
        {
            GameData.Instance.hackerIsReady = false;
            GameData.Instance.masterIsReady = false;
            GameData.Instance.currentLevel++;
            SteamVR_LoadLevel.Begin("game");
        }
        pointsText.text = "You can still spend " + points + " Points.";
    }

    public void gruntUpgradeUnlock()
    {
        if (points <= 0)
            return;
        points--;
        GameData.Instance.betterRangedGruntUnlocked = true;
        gruntUpgrade.interactable = false;
    }

    public void suicideRobotUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.suicideRobotUnlocked = true;
        suicideRobot.interactable = false;
    }

    public void sniperUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.sniperUnlocked = true;
        sniper.interactable = false;
    }

    public void turretUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.turretUnlocked = true;
        turret.interactable = false;
    }

    public void hackingAreaUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.hackingAreaUnlocked = true;
        hackingArea.interactable = false;
    }

    public void firewallUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.firewallUnlocked = true;
        firewall.interactable = false;
    }

    public void fragmentationSawUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.fragmentationSawUnlocked = true;
        fragmentationSaw.interactable = false;
    }
}
