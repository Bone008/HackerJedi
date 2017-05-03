using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LaserInteractableButton : MonoBehaviour, ILaserInteractable
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void Activate()
    {
        // TODO
    }

    public void SetHovered(bool flag)
    {
        // TODO no idea if this works
        if(flag) button.OnPointerEnter(null);
        else button.OnPointerExit(null);
    }
}
