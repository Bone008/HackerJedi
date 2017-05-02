using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour {

    [Header("Buttons")]
    #region
    public GameObject suicideRobot;
    public GameObject sniper;
    public GameObject turret;
    public GameObject hackingArea;
    public GameObject firewall;
    #endregion

    public Text text;

    private int points;

    // TODO place logic of "hacker and master both have to press continue" here
    void Start()
    {
        points = 2;

        GameData.Instance.hackerIsReady = GameData.Instance.masterIsReady = false;
        if (!GameData.Instance.viveActive)
            GameData.Instance.hackerIsReady = true;

        #region
        if (GameData.Instance.suicideRobotUnlocked)
            suicideRobot.SetActive(false);
        if (GameData.Instance.sniperUnlocked)
            sniper.SetActive(false);
        if (GameData.Instance.turretUnlocked)
            turret.SetActive(false);
        if (GameData.Instance.hackingAreaUnlocked)
            hackingArea.SetActive(false);
        if (GameData.Instance.firewallUnlocked)
            firewall.SetActive(false);
        #endregion
    }

    public void hackerReady()
    {
        GameData.Instance.hackerIsReady = true;
    }

    public void masterReady()
    {
        GameData.Instance.masterIsReady = true;
    }

    public void Update()
    {
        if(GameData.Instance.hackerIsReady && GameData.Instance.masterIsReady && !SteamVR_LoadLevel.loading)
            SteamVR_LoadLevel.Begin("game");
        text.text = "You still can spent " + points + " Points.";
    }

    public void suicideRobotUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.suicideRobotUnlocked = true;
        suicideRobot.SetActive(false);
    }

    public void sniperUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.sniperUnlocked = true;
        sniper.SetActive(false);
    }

    public void turretUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.turretUnlocked = true;
        turret.SetActive(false);
    }

    public void hackingAreaUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.hackingAreaUnlocked = true;
        hackingArea.SetActive(false);
    }

    public void firewallUnlock()
    {
        if (points == 0)
            return;
        points--;
        GameData.Instance.firewallUnlocked = true;
        firewall.SetActive(false);
    }
}
