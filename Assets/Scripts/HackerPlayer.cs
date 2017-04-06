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


    public float maxHealth = 100.0f;
    private float currentHealth;

    // one entry per hand
    private Dictionary<AbilityType, GameObject>[] allAbilityGOs = { new Dictionary<AbilityType, GameObject>(), new Dictionary<AbilityType, GameObject>() };
    private AbilityType[] equippedAbilities = { AbilityType.None, AbilityType.None };
    private AbilitySelectionWheel[] selectionWheels = { null, null };


    private GameObject GetHandGO(HackerHand hand)
    {
        return handGameObjects[(int)hand];
        //return (hand == HackerHand.Left ? leftHand : rightHand);
    }

    private void Start()
    {
        Debug.Assert(handGameObjects.Length == 2);

        currentHealth = maxHealth;
        SpawnAbilityInstances();
        EquipAbility(HackerHand.Left, initialAbilityLeft);
        EquipAbility(HackerHand.Right, initialAbilityRight);
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

                allAbilityGOs[i].Add(go.GetComponent<IAbility>().Type, go);
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
            allAbilityGOs[i][equippedAbilities[i]].SetActive(false);
        }

        // enable new equipment
        allAbilityGOs[i][type].SetActive(true);
        equippedAbilities[i] = type;
    }

    public void Fire(HackerHand hand)
    {
        allAbilityGOs[(int)hand][equippedAbilities[(int)hand]].SendMessage("Fire", SendMessageOptions.DontRequireReceiver);
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
        AbilitySelectionWheel wheel = selectionWheels[(int)hand];
        if (wheel == null)
            return; // not open

        AbilityType? newAbility = wheel.ConfirmSelection(position);
        if(newAbility != null)
        {
            EquipAbility(hand, newAbility.Value);
        }
    }

    public void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth < 0)
        {
            Debug.Log("You Died!");
            // TODO
        }
    }
}
