using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackerProgressionManager : MonoBehaviour
{
    public int upgradePointsPerLevel = 3;

    public Text upgradePointsText;
    public Text levelText;

    void Start()
    {
        HackerProgression.Instance.AwardUpgradePoints(upgradePointsPerLevel);
        RefreshUpgradeStates();

        levelText.text = string.Format("Stage {0} complete!", GameData.Instance.currentLevel);

        // attach event listeners
        foreach (var button in GetComponentsInChildren<AbilityUpgradeElement>())
            button.onUnlockClick.AddListener(() => TryBuyUpgrade(button));
    }

    private void RefreshUpgradeStates()
    {
        upgradePointsText.text = "" + HackerProgression.Instance.PointsToSpend;

        foreach (var button in GetComponentsInChildren<AbilityUpgradeElement>())
        {
            int unlockedLevel = HackerProgression.Instance.GetUnlockedLevel(button.abilityType);

            if (unlockedLevel >= button.level)
                button.SetState(AbilityUpgradeElement.State.Purchased);
            else if (HackerProgression.Instance.PointsToSpend == 0 || unlockedLevel < button.level - 1)
                button.SetState(AbilityUpgradeElement.State.Disabled);
            else
                button.SetState(AbilityUpgradeElement.State.Default);
        }
    }

    public void TryBuyUpgrade(AbilityUpgradeElement button)
    {
        if (HackerProgression.Instance.GetUnlockedLevel(button.abilityType) == button.level - 1)
        {
            if (HackerProgression.Instance.TryBuyUpgrade(button.abilityType))
            {
                Debug.Log("Hacker: Bought " + button.abilityType + " level " + button.level);
                RefreshUpgradeStates();
            }
            else
            {
                Debug.Log("Hacker: Failed to buy " + button.abilityType + " level " + button.level);
            }
        }
    }

}
