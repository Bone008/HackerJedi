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
        // hack
        button.onClick.Invoke();
    }

    public void SetHovered(bool flag)
    {
        // hack
        if (flag) button.OnPointerEnter(null);
        else button.OnPointerExit(null);
    }
}
