using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectionTrigger : MonoBehaviour {

    public AbilitySelectionWheel wheel;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter", other);
        var element = other.GetComponent<AbilitySelectionElement>() ?? other.GetComponentInParent<AbilitySelectionElement>();
        if(element != null)
            wheel.SelectedElement = element;
    }

    private void OnTriggerExit(Collider other)
    {
        var element = other.GetComponent<AbilitySelectionElement>() ?? other.GetComponentInParent<AbilitySelectionElement>();
        if(wheel.SelectedElement == element)
            wheel.SelectedElement = null;
    }
}
