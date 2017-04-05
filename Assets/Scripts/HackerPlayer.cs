using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HackerHand { Left = 0, Right = 1 }

public class HackerPlayer : MonoBehaviour
{

    public GameObject abilitySelectionPrefab;
    public GameObject leftHand;
    public GameObject rightHand;

    // will be made selectable soon
    public Gun leftGun;
    public Gun rightGun;

    public float maxHealth = 100.0f;
    private float currentHealth;

    private GameObject[] selectionWheels = { null, null };

    private GameObject GetHandGO(HackerHand hand)
    {
        return (hand == HackerHand.Left ? leftHand : rightHand);
    }


    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    public void Fire(HackerHand hand)
    {
        if (hand == HackerHand.Left)
        {
            if (leftGun != null) leftGun.Fire();
        }
        else
        {
            if (rightGun != null) rightGun.Fire();
        }
    }

    public void OpenAbilitySelectionWheel(HackerHand hand)
    {
        Debug.Log("open " + hand);
        if (selectionWheels[(int)hand] != null)
            // already open
            return;

        var go = Instantiate(abilitySelectionPrefab);
        go.transform.SetParent(GetHandGO(hand).transform, false);
        selectionWheels[(int)hand] = go;
    }

    public void CloseAbilitySelectionWheel(HackerHand hand)
    {
        if (selectionWheels[(int)hand] == null)
            // already closed
            return;

        Destroy(selectionWheels[(int)hand]);
        selectionWheels[(int)hand] = null;
    }

    public void ConfirmAbilitySelection(HackerHand hand)
    {
        // TODO
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
