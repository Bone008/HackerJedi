using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityUpgradeElement : MonoBehaviour, ILaserInteractable
{
    public enum State
    {
        Default,
        Hovered,
        Purchased,
        Disabled,
    }

    public Renderer sphereRenderer;
    public Color hoveredColor;
    public Color disabledColor;
    public Color purchasedColor;

    public AbilityType abilityType;
    public int level;

    [HideInInspector]
    public UnityEvent onTriggered = new UnityEvent();

    private State state;
    private bool StateIsActive { get { return state == State.Default || state == State.Hovered; } }

    private Color initialColor;

    private void Start()
    {
        initialColor = sphereRenderer.material.color;
    }

    public void SetState(State state)
    {
        if (this.state == state)
            return; // no chagne

        Color targetColor;
        switch(state)
        {
            case State.Default: targetColor = initialColor; break;
            case State.Disabled: targetColor = disabledColor; break;
            case State.Hovered: targetColor = hoveredColor; break;
            case State.Purchased: targetColor = purchasedColor; break;
            default: throw new ArgumentException("unknown state: " + state);
        }
        this.state = state;

        this.Animate(0.2f, sphereRenderer.material.color, targetColor, Color.Lerp, c => sphereRenderer.material.color = c);
    }

    public void Activate()
    {
        if (!StateIsActive)
            return;

        onTriggered.Invoke();
    }

    public void SetHovered(bool flag)
    {
        if (StateIsActive)
            SetState(flag ? State.Hovered : State.Default);
    }
}
