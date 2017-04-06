using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySelectionWheel : MonoBehaviour
{

    public AbilitySelectionTrigger selectionSphere;
    public float selectionSphereMultiplier;
    public GameObject[] abilityPrefabs;
    public float initAnimationDuration;

    // this is updated automatically by the selectionPreview by its collider trigger
    public AbilitySelectionElement SelectedElement { get; set; }

    void Awake()
    {
        Debug.Assert(abilityPrefabs != null && abilityPrefabs.Length > 0, "no ability prefabs configured", gameObject);
    }

    void Start()
    {
        StartCoroutine(SpawnAnimation());
    }


    private IEnumerator SpawnAnimation()
    {
        var elements = GetComponentsInChildren<AbilitySelectionElement>();
        var animatedElements = elements.Select(e => new
        {
            e.transform,
            collider = e.GetComponentInChildren<SphereCollider>(),
            initialPosition = e.transform.localPosition
        }).ToArray();


        float t = 0;
        while (t < initAnimationDuration)
        {
            foreach (var e in animatedElements)
            {
                Vector3 inverseScale = e.collider.transform.localScale;
                inverseScale.x = 1 / inverseScale.x;
                inverseScale.y = 1 / inverseScale.y;
                inverseScale.z = 1 / inverseScale.z;

                e.transform.localPosition = (t / initAnimationDuration) * e.initialPosition;
                e.collider.center = Vector3.Scale((1 - (t / initAnimationDuration)) * e.initialPosition, inverseScale);
            }

            yield return null;
            t += Time.deltaTime;
        }

        foreach (var e in animatedElements)
        {
            e.transform.localPosition = e.initialPosition;
            e.collider.center = Vector3.zero;
        }
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
