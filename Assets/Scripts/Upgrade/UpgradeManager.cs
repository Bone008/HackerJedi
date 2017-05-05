using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour {

    [Header("Buttons")]
    #region
    public Button suicideRobot;
    public Button sniper;
    public Button turret;
    public Button hackingArea;
    public Button firewall;
    #endregion

    public Text levelText;
    public Text pointsText;

    private int points;
    
    void Start()
    {
        points = 2;

        GameData.Instance.hackerIsReady = GameData.Instance.masterIsReady = false;

        levelText.text = string.Format("Hacker has completed stage {0}!", GameData.Instance.currentLevel);

        #region
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
        #endregion
    }

    public void hackerReady()
    {
        Debug.Log("Upgrade: hacker is ready");
        GameData.Instance.hackerIsReady = true;
    }

    public void masterReady()
    {
        Debug.Log("Upgrade: hacker is ready");
        GameData.Instance.masterIsReady = true;
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
}
