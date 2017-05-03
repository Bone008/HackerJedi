using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackerProgressionManager : MonoBehaviour
{

    public AbilityUpgradeElement[] upgradeButtons;

    void Start()
    {
        foreach (var button in upgradeButtons)
        {
            if (HackerProgression.Instance.GetUnlockedLevel(button.abilityType) >= button.level)
                button.gameObject.SetActive(false);
            else
                button.onTriggered.AddListener(() => TryBuyUpgrade(button));
        }
    }

    public void TryBuyUpgrade(AbilityUpgradeElement button)
    {
        if (HackerProgression.Instance.GetUnlockedLevel(button.abilityType) == button.level - 1)
        {
            if (HackerProgression.Instance.TryBuyUpgrade(button.abilityType))
                button.gameObject.SetActive(false);
        }
    }
}
