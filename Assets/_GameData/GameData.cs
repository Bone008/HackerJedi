using System.Collections;
using System.Collections.Generic;
using System;

public class GameData
{
    private static GameData instance;

    public bool isPaused = false;
    public bool hackerIsReady;
    public bool masterIsReady;

    public int randomizeWorldinProgress = 0;
    public int levels = 3;

    private GameData()
    {
        if (instance != null)
            return;
        instance = this;
    }

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