using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DoAnimation());
    }

    private IEnumerator DoAnimation()
    {
        yield return this.AnimateVector(0.6f, Vector3.zero, 10 * Vector3.one, s => transform.localScale = s);
        Destroy(gameObject);
    }
}
