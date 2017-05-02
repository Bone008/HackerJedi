using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Placeables : MonoBehaviour {

    public GameObject[] buttonGOs;
    public PlaceableBase[] placeables;

    private SpawnResource spawnResource;
    private Button[] buttons;

	void Start () {
        // should be pairs
        Debug.Assert(buttonGOs.Length == placeables.Length);

        // get spawn resources
        spawnResource = GetComponent<SpawnResource>();

        // transfer to needed components
        buttons = new Button[buttonGOs.Length];
        for (int i = 0; i < buttonGOs.Length; i++)
        {
            buttons[i] = buttonGOs[i].GetComponent<Button>();
        }

        // write cost on buttons
        for (int i = 0; i < buttonGOs.Length; i++)
        {
            float cost = placeables[i].placingCost;
            buttonGOs[i].GetComponentInChildren<Text>().text += string.Format(" ({0}$)", (int)cost);
        }
    }

    void Update()
    {
        // disable/enable all buttons depending on if the placeable can be bought
        for(int i = 0; i < buttons.Length; i++)
        {
            float cost = placeables[i].placingCost;
            buttons[i].interactable = (cost <= spawnResource.currentValue);
        }
    }

}
