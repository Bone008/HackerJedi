using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectionTrigger : MonoBehaviour {

    public AbilitySelectionWheel wheel;
    public Material materialRegular;
    public Material materialHighlight;

    private void OnTriggerEnter(Collider other)
    {
        var element = other.GetComponent<AbilitySelectionElement>() ?? other.GetComponentInParent<AbilitySelectionElement>();
        if (element != null)
        {
            if(wheel.SelectedElement != null)
            {
                wheel.SelectedElement.GetComponentInChildren<Renderer>().sharedMaterial = materialRegular;
            }

            wheel.SelectedElement = element;
            other.GetComponentInChildren<Renderer>().sharedMaterial = materialHighlight;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var element = other.GetComponent<AbilitySelectionElement>() ?? other.GetComponentInParent<AbilitySelectionElement>();
        if (wheel.SelectedElement == element)
        {
            wheel.SelectedElement = null;
            other.GetComponentInChildren<Renderer>().sharedMaterial = materialRegular;
        }
    }
}
