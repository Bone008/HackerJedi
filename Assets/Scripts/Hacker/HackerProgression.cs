using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>A singleton that stores progression (unlocked abilities) of the hacker.</summary>
public class HackerProgression
{
    private static readonly Dictionary<AbilityType, int> maxLevels = new Dictionary<AbilityType, int>
    {
        {AbilityType.SimpleGun, 3},
        {AbilityType.JediForcePush, 2},
        {AbilityType.PulseGun, 2},
        {AbilityType.Shield, 2},
        // ...
    };


    private int pointsToSpend = 0;
    private Dictionary<AbilityType, int> unlockedLevels = new Dictionary<AbilityType, int>();

    public int PointsToSpend { get { return pointsToSpend; } }

    public void Reset()
    {
        unlockedLevels.Clear();
        unlockedLevels.Add(AbilityType.SimpleGun, 1);
        unlockedLevels.Add(AbilityType.JediForcePush, 1);
        unlockedLevels.Add(AbilityType.PulseGun, 1);
        unlockedLevels.Add(AbilityType.Shield, 1);
    }

    private HackerProgression()
    {
        Reset();

        // for debugging: unlock everything
        //unlockedLevels[AbilityType.SimpleGun] = maxLevels[AbilityType.SimpleGun];
        unlockedLevels[AbilityType.JediForcePush] = maxLevels[AbilityType.JediForcePush];
        //unlockedLevels[AbilityType.PulseGun] = maxLevels[AbilityType.PulseGun];
    }


    public int GetUnlockedLevel(AbilityType ability)
    {
        if (unlockedLevels.ContainsKey(ability))
            return unlockedLevels[ability];
        else
            return 0;
    }


    // ultimate is unlocked when the max level is reached on an ability
    public bool IsUltimateUnlocked(AbilityType ability)
    {
        if (!maxLevels.ContainsKey(ability))
            throw new ArgumentException("ability " + ability + " cannot be checked for ultimate availability: no max level");

        return GetUnlockedLevel(ability) >= maxLevels[ability];
    }


    // (called upon level completion)
    public void AwardUpgradePoints(int amount)
    {
        pointsToSpend += amount;
    }

    // (called when hacker clicks upgrade button)
    public bool TryBuyUpgrade(AbilityType ability)
    {
        if (pointsToSpend <= 0)
            return false;

        pointsToSpend--;
        if (unlockedLevels.ContainsKey(ability))
        {
            if (unlockedLevels[ability] >= maxLevels[ability])
                return false; // already at max level

            unlockedLevels[ability]++;
        }
        else
            unlockedLevels[ability] = 1;

        return true;
    }




    // ==== Singleton pattern ====

    private static HackerProgression instance = null;

    public static HackerProgression Instance
    {
        get
        {
            if (instance == null) instance = new HackerProgression();
            return instance;
        }
    }
}
