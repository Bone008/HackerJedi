using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEye : MonoBehaviour {

    public GameObject deathExplosion;
    
    public void OnDeath(Damageable deadEye)
    {
        // TODO
        Debug.Log("master deadeded");
        deadEye.RestoreFullHealth();
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
    }
    
}
