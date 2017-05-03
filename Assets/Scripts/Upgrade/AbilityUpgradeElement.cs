using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityUpgradeElement : MonoBehaviour, ILaserInteractable
{
    public Color hoveredColor;

    public AbilityType abilityType;
    public int level;

    [HideInInspector]
    public UnityEvent onTriggered = new UnityEvent();

    private Renderer sphereRenderer;
    private Color initialColor;

    private void Start()
    {
        sphereRenderer = GetComponentInChildren<Renderer>();
        initialColor = sphereRenderer.material.color;
    }

    public void Activate()
    {
        onTriggered.Invoke();
    }

    public void SetHovered(bool flag)
    {
        var targetColor = (flag ? hoveredColor : initialColor);
        this.Animate(0.2f, sphereRenderer.material.color, targetColor, Color.Lerp, c => sphereRenderer.material.color = c);
    }
}
