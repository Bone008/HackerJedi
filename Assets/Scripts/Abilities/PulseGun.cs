using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class PulseGun : AbstractAbility {

    public AudioClip shootSound;
    public AudioClip emptySound;
    public GameObject particlePrefab;
    public Transform nozzle;
    public float shootCooldown;
    public float totalAngle;
    public float maxRange;
    public float maxDamageAmount;
    public AnimationCurve damageFalloff = AnimationCurve.Linear(0, 1.0f, 1, 0.2f);
    public LayerMask hitLayerMask;

    //public Color volumetricLineColor;
    public Material volumetricLineMaterial;

    [HideInInspector]
    public int? layer = null;

    private void Start()
    {
        if(!layer.HasValue)
            layer = gameObject.layer;
    }

    protected override void OnTriggerDown()
    {
        if (IsCoolingDown)
        {
            if (emptySound != null)
            {
                var go = new GameObject("Empty sound");
                go.transform.position = nozzle.position;
                var audio = go.AddComponent<AudioSource>();
                audio.clip = emptySound;
                audio.volume = 0.3f;
                audio.Play();
                this.Delayed(emptySound.length + 0.5f, () => Destroy(go));
            }
            return;
        }
            
        CooldownFor(shootCooldown);


        // shooting
        var aimRay = GetAimRay(nozzle);
        
        Collider[] colliders = Physics.OverlapSphere(nozzle.position, maxRange, hitLayerMask);
        foreach(var coll in colliders)
        {
            // instead of using the transform origin of the target, use the point that was actually hit on the collider
            Vector3 pointHit = coll.ClosestPointOnBounds(nozzle.position);
            Vector3 nozzleToPoint = pointHit - nozzle.position;

            // only include stuff in a cone in front of the nozzle
            if (Vector3.Angle(nozzleToPoint, aimRay.direction) > totalAngle / 2)
                continue;

            HealthResource health = coll.gameObject.GetComponentInParent<HealthResource>();
            // only include stuff that can be damaged
            if (health == null)
                return;

            // sanity check
            if (nozzleToPoint.magnitude > maxRange)
                Debug.LogWarning("math has broken");

            // 0 = point blank range; 1 = max range
            float distanceFalloffFactor = nozzleToPoint.magnitude / maxRange;
            float damage = maxDamageAmount * damageFalloff.Evaluate(distanceFalloffFactor);

            Debug.Log("dist=" + distanceFalloffFactor + "; dmg=" + damage, coll.gameObject);

            health.ChangeValue(-damage);
            //coll.gameObject.SetActive(false);
            //this.Delayed((damage / maxDamageAmount), () => coll.gameObject.SetActive(true));
        }

        
        // visual effect
        int nPoints = 100;
        Vector3[] vertices = new Vector3[nPoints + 1];
        vertices[0] = nozzle.position;
        float tanAlpha = Mathf.Tan(Mathf.Deg2Rad * totalAngle / 2.0f);
        for(int i=1; i<=nPoints; i++)
        {
            float r = UnityEngine.Random.Range(0.0f, 1.0f);
            float randZ = (1 - r*r) * maxRange;
            Vector2 randXY = UnityEngine.Random.insideUnitCircle * randZ * tanAlpha;
            Vector3 localPos = new Vector3(randXY.x, randXY.y, randZ);

            vertices[i] = nozzle.TransformPoint(localPos);
        }

        GameObject fxGO = new GameObject("PulseGun FX");
        fxGO.AddComponent<MeshFilter>();
        fxGO.AddComponent<MeshRenderer>().sharedMaterial = volumetricLineMaterial;
        var lineScript = fxGO.AddComponent<VolumetricLines.VolumetricLineStripBehavior>();
        lineScript.SetLineColorAtStart = true;
        lineScript.LineColor = volumetricLineMaterial.color;
        lineScript.LineWidth = 0.12f;
        lineScript.UpdateLineVertices(vertices);
        StartCoroutine(AnimateFX(lineScript));


        if (shootSound != null)
            AudioSource.PlayClipAtPoint(shootSound, nozzle.position, 0.5f);
    }

    private IEnumerator AnimateFX(VolumetricLineStripBehavior lineScript)
    {
        float maxLineWidth = lineScript.LineWidth;

        yield return this.AnimateScalar(0.07f, 0, maxLineWidth, v => lineScript.LineWidth = v);
        yield return this.AnimateScalar(0.15f, maxLineWidth, 0, v => lineScript.LineWidth = v);
        
        Destroy(lineScript.gameObject);
    }
}
