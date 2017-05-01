using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {
    
    // TODO place logic of "hacker and master both have to press continue" here

    public void Continue()
    {
        SteamVR_LoadLevel.Begin("game");
    }

}
