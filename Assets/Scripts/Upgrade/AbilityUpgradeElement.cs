using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityUpgradeElement : MonoBehaviour, ILaserInteractable
{
    public enum State
    {
        Default,
        Hovered,
        Purchased,
        Disabled,
    }

    // needed to ensure that only one ability is previewed at a time
    private static AbilityUpgradeElement currentlyPreviewingScript = null;

    [Header("Generic prefab settings")]
    public Renderer sphereRenderer;
    public Color hoveredColor;
    public Color disabledColor;
    public Color purchasedColor;

    public Transform detailsCanvas;
    public Text detailsNameUI;
    public Text detailsLevelUI;
    public Text detailsDescriptionUI;
    public Button detailsUnlockUI;

    [Tooltip("location offset when the bubble is clicked once")]
    public Vector3 previewOffset;
    public float previewTransitionTime1;
    public float previewTransitionTime2;

    [Header("Bubble specific")]
    public AbilityType abilityType;
    public int level;
    public string detailsName;
    [TextArea]
    public string detailsDescription;

    [HideInInspector]
    public UnityEvent onUnlockClick = new UnityEvent();

    private State state = State.Default;
    private bool StateIsActive { get { return state == State.Default || state == State.Hovered; } }
    private bool isPreviewing = false;
    private Coroutine previewTransition = null;

    private Color initialColor;
    private Vector3 initialPosition;
    private Vector3 initialCanvasScale;

    private void Start()
    {
        initialColor = sphereRenderer.material.color;
        initialPosition = transform.localPosition;
        initialCanvasScale = detailsCanvas.localScale;

        // set details texts
        detailsNameUI.text = detailsName;
        detailsLevelUI.text = "Level " + level;
        detailsDescriptionUI.text = detailsDescription;
    }

    public void SetState(State state)
    {
        if (this.state == state)
            return; // no change
        this.state = state;

        Color targetColor;
        string buttonText;
        switch (state)
        {
            case State.Default: targetColor = initialColor; buttonText = "Unlock"; break;
            case State.Disabled: targetColor = disabledColor; buttonText = "cannot unlock"; break;
            case State.Hovered: targetColor = hoveredColor; buttonText = "Unlock"; break;
            case State.Purchased: targetColor = purchasedColor; buttonText = "Unlocked"; break;
            default: throw new ArgumentException("unknown state: " + state);
        }
        
        // change unlock button
        detailsUnlockUI.interactable = StateIsActive;
        detailsUnlockUI.GetComponentInChildren<Text>().text = buttonText;

        var cols = detailsUnlockUI.colors;
        if (state == State.Disabled)
            cols.disabledColor = disabledColor;
        else if(state == State.Purchased)
            cols.disabledColor = purchasedColor;
        detailsUnlockUI.colors = cols;

        // change sphere color
        this.Animate(0.2f, sphereRenderer.material.color, targetColor, Color.Lerp, c => sphereRenderer.material.color = c);
    }

    public void Activate()
    {
        if (previewTransition != null || (currentlyPreviewingScript != null && currentlyPreviewingScript.previewTransition != null))
            return;

        if(isPreviewing)
        {
            Debug.Assert(currentlyPreviewingScript == this);

            isPreviewing = false;
            currentlyPreviewingScript = null;
            previewTransition = StartCoroutine(ClosePreview());
        }
        else
        {
            if (currentlyPreviewingScript != null)
                currentlyPreviewingScript.Activate(); // make it close

            isPreviewing = true;
            currentlyPreviewingScript = this;
            previewTransition = StartCoroutine(OpenPreview());
        }
    }

    public void Unlock()
    {
        if (StateIsActive)
        {
            // confirm buy
            onUnlockClick.Invoke();
        }
    }

    private IEnumerator OpenPreview()
    {
        // move to foreground
        yield return this.AnimateVector(previewTransitionTime1, initialPosition, initialPosition + previewOffset, Util.EaseInOut01, p => transform.localPosition = p);

        // show details panel
        detailsCanvas.gameObject.SetActive(true);
        Vector3 scaleStart = initialCanvasScale;
        scaleStart.y = 0;
        yield return this.AnimateVector(previewTransitionTime2, scaleStart, initialCanvasScale, Util.EaseInOut01,  s => detailsCanvas.localScale = s);

        previewTransition = null;
    }

    private IEnumerator ClosePreview()
    {
        // hide details panel
        Vector3 scaleEnd = initialCanvasScale;
        scaleEnd.y = 0;
        yield return this.AnimateVector(previewTransitionTime2, initialCanvasScale, scaleEnd, Util.EaseInOut01, s => detailsCanvas.localScale = s);
        detailsCanvas.gameObject.SetActive(false);
        
        // move to background
        yield return this.AnimateVector(previewTransitionTime1, initialPosition + previewOffset, initialPosition, Util.EaseInOut01, p => transform.localPosition = p);
        
        previewTransition = null;
    }

    public void SetHovered(bool flag)
    {
        if (StateIsActive)
            SetState(flag ? State.Hovered : State.Default);
    }

}
