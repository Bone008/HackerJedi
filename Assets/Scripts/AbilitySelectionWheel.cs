using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectionWheel : MonoBehaviour {

    public Transform selectionPreview;

    public AbilitySelectionElement SelectedElement { get; set; }

    public void SetPreviewPosition(Vector2 position)
    {
        selectionPreview.transform.localPosition = position * 12;
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
