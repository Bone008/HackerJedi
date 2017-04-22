using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HackerHand { Left = 0, Right = 1 }

public class HackerPlayer : MonoBehaviour
{
    public GameObject[] handGameObjects = new GameObject[2];

    public GameObject abilitySelectionPrefab;
    public AbilityType initialAbilityLeft;
    public AbilityType initialAbilityRight;

    [Header("UI")]
    public RectTransform healthBarPanel;
    public RectTransform dataFragmentsPanel;
    public GameObject deathScreenElement;

    // one entry per hand
    private Dictionary<AbilityType, GameObject>[] allAbilityGOs = { new Dictionary<AbilityType, GameObject>(), new Dictionary<AbilityType, GameObject>() };
    private AbilityType[] equippedAbilities = { AbilityType.None, AbilityType.None };
    private AbilitySelectionWheel[] selectionWheels = { null, null };


    public GameObject GetHandGO(HackerHand hand)
    {
        return handGameObjects[(int)hand];
        //return (hand == HackerHand.Left ? leftHand : rightHand);
    }

    private GameObject GetEquippedAbilityGO(HackerHand hand)
    {
        GameObject go;
        if (allAbilityGOs[(int)hand].TryGetValue(equippedAbilities[(int)hand], out go))
            return go;
        else
            return null;
    }

    private AbstractAbility GetEquippedAbilityScript(HackerHand hand)
    {
        GameObject go = GetEquippedAbilityGO(hand);
        if (go != null)
            return go.GetComponent<AbstractAbility>(); // may also return null if the ability has no script (yet)
        else
            return null;
    }


    private void Start()
    {
        Debug.Assert(handGameObjects.Length == 2);

        SpawnAbilityInstances();
        EquipAbility(HackerHand.Left, initialAbilityLeft);
        EquipAbility(HackerHand.Right, initialAbilityRight);

        healthBarPanel.transform.localScale = new Vector3(1, 1, 1);
        deathScreenElement.SetActive(false);
    }

    // instantiates all ability GOs (disabled) for each hand and stores them in allAbilityGOs
    private void SpawnAbilityInstances()
    {
        // for both hands
        for (int i = 0; i < 2; i++)
        {
            HackerHand hand = (HackerHand)i;

            var abilityPrefabs = abilitySelectionPrefab.GetComponent<AbilitySelectionWheel>().abilityPrefabs;
            foreach (var prefab in abilityPrefabs)
            {
                var go = Instantiate(prefab);
                go.SetActive(false);
                go.transform.SetParent(GetHandGO(hand).transform, false);

                allAbilityGOs[i].Add(go.GetComponent<AbstractAbility>().Type, go);
            }
        }
    }



    private void EquipAbility(HackerHand hand, AbilityType type)
    {
        int i = (int)hand;
        Debug.Assert(0 <= i && i <= 1);

        if (!allAbilityGOs[i].ContainsKey(type))
        {
            Debug.LogWarning("cannot equip ability type, no prefab configured: " + type);
            return;
        }


        // disable old equipment
        if (allAbilityGOs[i].ContainsKey(equippedAbilities[i]))
        {
            var script = GetEquippedAbilityScript(hand);
            if (script != null)
            {
                // make sure that we stop firing before disabling
                script.SetTriggerDown(false);
                script.SetGripDown(false);
            }
            allAbilityGOs[i][equippedAbilities[i]].SetActive(false);
        }

        // enable new equipment
        allAbilityGOs[i][type].SetActive(true);
        equippedAbilities[i] = type;
    }

    // called by the concrete input handler when the trigger state has changed
    public void SetTriggerDown(HackerHand hand, bool state)
    {
        var abilityScript = GetEquippedAbilityScript(hand);
        if (abilityScript != null)
            abilityScript.SetTriggerDown(state);
    }

    // called by the concrete input handler when the grip state has changed
    public void SetGripDown(HackerHand hand, bool state)
    {
        var abilityScript = GetEquippedAbilityScript(hand);
        if (abilityScript != null)
            abilityScript.SetGripDown(state);
    }


    public void OpenAbilitySelectionWheel(HackerHand hand)
    {
        if (selectionWheels[(int)hand] != null)
            return; // already open

        var go = Instantiate(abilitySelectionPrefab);
        go.transform.SetParent(GetHandGO(hand).transform, false);
        selectionWheels[(int)hand] = go.GetComponent<AbilitySelectionWheel>();
    }

    public void CloseAbilitySelectionWheel(HackerHand hand)
    {
        AbilitySelectionWheel wheel = selectionWheels[(int)hand];

        if (wheel == null)
            return; // already closed

        Destroy(wheel.gameObject);
        selectionWheels[(int)hand] = null;
    }

    public void SetAbilitySelectionPosition(HackerHand hand, Vector2 position)
    {
        AbilitySelectionWheel wheel = selectionWheels[(int)hand];
        if (wheel == null)
            return; // not open

        wheel.SetPreviewPosition(position);
    }

    public void ConfirmAbilitySelection(HackerHand hand, Vector2 position)
    {
        StartCoroutine(_ConfirmCoroutineHelper(hand, position));
    }

    private IEnumerator _ConfirmCoroutineHelper(HackerHand hand, Vector2 position)
    {
        yield return null; // delay by one frame

        AbilitySelectionWheel wheel = selectionWheels[(int)hand];
        if (wheel == null)
            yield break; // not open

        AbilityType? newAbility = wheel.ConfirmSelection(position);
        if(newAbility != null)
        {
            EquipAbility(hand, newAbility.Value);
            //CloseAbilitySelectionWheel(hand);
        }
    }

    public void OnDeath(Damageable deadHacker)
    {
        // TODO
        Debug.Log("You deaded!");
        deadHacker.RestoreFullHealth();
        
        deathScreenElement.SetActive(true);
        this.Delayed(2.0f, () => deathScreenElement.SetActive(false));
    }

    public void OnHealthChanged(Damageable hacker)
    {
        healthBarPanel.transform.localScale = new Vector3(hacker.healthPercentage, 1, 1);
    }

    public void OnDataFragmentsChanged(DataFragmentResource resource)
    {
        dataFragmentsPanel.transform.localScale = new Vector3(resource.filledPercentage, 1, 1);
    }
}
