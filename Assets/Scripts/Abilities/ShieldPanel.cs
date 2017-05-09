using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShieldPanel : MonoBehaviour
{

    public Transform[] panels;
    public Color highlightColor;

    private Color initialColor;
    private Collider myCollider;

    // Use this for initialization
    void Start()
    {
        myCollider = GetComponent<Collider>();
        initialColor = panels[0].GetComponent<Renderer>().sharedMaterial.GetColor("_EmissionColor");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision!");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("test: " + other.name + "; " + other.gameObject.name, other);
        var projectile = other.GetComponent<Projectile>();
        if (projectile != null)
        {
            // this kills the projectile
            Destroy(other.gameObject);

            // highlight panels
            var collPos = other.transform.position;
            List<Transform> panelsInOrder = panels.OrderBy(p => (p.position - collPos).sqrMagnitude).ToList();
            float minDist = (panelsInOrder[0].position - collPos).magnitude;
            float maxDist = (panelsInOrder.Last().position - collPos).magnitude;
            for (int i = 0; i < panelsInOrder.Count; i++)
            {
                var panel = panels[i];
                var mat = panel.GetComponent<Renderer>().material;

                float dist = (panel.position - collPos).magnitude;

                this.Delayed(0.4f * (dist - minDist) / (maxDist - minDist), () =>
                {
                    mat.SetColor("_EmissionColor", highlightColor);
                    this.Animate(0.3f, highlightColor, initialColor, Color.Lerp, Util.EaseInOut01, c => mat.SetColor("_EmissionColor", c));
                    //this.Delayed(0.1f, () => mat.SetColor("_EmissionColor", initialColor));
                });
            }


            //var prim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //prim.transform.position = other.transform.position;
            //prim.transform.localScale = 0.05f * Vector3.one;
            //prim.transform.SetParent(transform, true);

            // unsuccessful attempts to deflect the bullet here :(

            //var otherLocalPos = transform.InverseTransformPoint(other.transform.position);
            //otherLocalPos.z = myCollider.bounds.max.z + 0.01f;

            //other.transform.position = transform.TransformPoint(otherLocalPos);
            //other.transform.rotation = Quaternion.LookRotation(transform.forward);

            //var rb = other.GetComponent<Rigidbody>();
            ////rb.velocity = Vector3.Reflect(rb.velocity, transform.forward);
            //rb.velocity = transform.forward * rb.velocity.magnitude;
            //this.Delayed(new WaitForFixedUpdate(), () => { rb.useGravity = true; rb.isKinematic = false; });
            //other.transform.position
            //Destroy(other.gameObject);
        }
    }
}
