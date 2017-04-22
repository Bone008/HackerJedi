using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEye : MonoBehaviour {

    public GameObject deathExplosion;
    public RectTransform healthBarPanel;
    
    public void OnDeath(HealthResource health)
    {
        // TODO
        Debug.Log("master deadeded");
        health.RestoreFullHealth();
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
    }
    
}
