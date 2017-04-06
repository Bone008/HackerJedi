using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectionWheel : MonoBehaviour {

    public Transform selectionPreview;
    public GameObject[] abilityPrefabs;

    // this is updated automatically by the selectionPreview by its collider trigger
    public AbilitySelectionElement SelectedElement { get; set; }

    void Awake()
    {
        Debug.Assert(abilityPrefabs != null && abilityPrefabs.Length > 0, "no ability prefabs configured", gameObject);
    }

    public void SetPreviewPosition(Vector2 position)
    {
        selectionPreview.transform.localPosition = position * 0.1f;
    }

    // returns the selected AbilityType, or null when the selector is not in contact with any button
    public AbilityType? ConfirmSelection(Vector2 position) // ignored
    {
        if (SelectedElement == null)
            return null;
        else
            return SelectedElement.abilityType;
    }
}
