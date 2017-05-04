using System.Collections;
using System.Collections.Generic;
using System;

public class GameData
{
    private static GameData instance;

    public bool isPaused = false;
    public bool hackerIsReady;
    public bool masterIsReady;

    public bool suicideRobotUnlocked = false;
    public bool sniperUnlocked = false;
    public bool turretUnlocked = false;
    public bool hackingAreaUnlocked = false;
    public bool firewallUnlocked = false;

    public bool viveActive = true;

    public int randomizeWorldinProgress = 0;
    public int levelCount = 3;
    public int currentLevel = 1;
    

    public static GameData Instance
    {
        get
        {
            if (instance == null)
                instance = new GameData();
            return instance;
        }
    }
}