using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySelectionWheel : MonoBehaviour
{

    public AbilitySelectionTrigger selectionSphere;
    public float selectionSphereMultiplier;
    public float initAnimationDuration;
    public GameObject[] abilityPrefabs;
    public GameObject[] ultimatePrefabs;

    // this is updated automatically by the selectionPreview by its collider trigger
    public AbilitySelectionElement SelectedElement { get; set; }

    void Awake()
    {
        Debug.Assert(abilityPrefabs != null && abilityPrefabs.Length > 0, "no ability prefabs configured", gameObject);
    }

    void Start()
    {
        StartSpawnAnimation();
    }


    private void StartSpawnAnimation()
    {
        var elements = GetComponentsInChildren<AbilitySelectionElement>();
        var animatedElements = elements.Select(e => new
        {
            e.transform,
            collider = e.GetComponentInChildren<SphereCollider>(),
            initialPosition = e.transform.localPosition
        }).ToArray();
        

        this.Animate(initAnimationDuration, progress =>
        {
            foreach (var e in animatedElements)
            {
                Vector3 inverseScale = e.collider.transform.localScale;
                inverseScale.x = 1 / inverseScale.x;
                inverseScale.y = 1 / inverseScale.y;
                inverseScale.z = 1 / inverseScale.z;

                e.transform.localPosition = progress * e.initialPosition;
                e.collider.center = Vector3.Scale((1 - progress) * e.initialPosition, inverseScale);
            }
        }, useRealtime: true);
    }


    public void SetPreviewPosition(Vector2 position)
    {
        selectionSphere.transform.localPosition = position * selectionSphereMultiplier;
    }

    // returns the selected AbilityType, or null when the selector is not in contact with any button
    public AbilityType? ConfirmSelection(Vector2 position) // ignored
    {
        SetPreviewPosition(position);

        // force a collider update
        var coll = selectionSphere.GetComponent<Collider>();
        coll.enabled = false;
        coll.enabled = true;


        if (SelectedElement == null)
            return null;
        else
            return SelectedElement.abilityType;
    }
}
