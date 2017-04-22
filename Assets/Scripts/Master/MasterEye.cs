using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEye : MonoBehaviour {

    public GameObject deathExplosion;
    public RectTransform healthBarPanel;
    
    public void OnDeath(Damageable deadEye)
    {
        // TODO
        Debug.Log("master deadeded");
        deadEye.RestoreFullHealth();
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
    }

    public void OnHealthChanged(Damageable eye)
    {
        Debug.Log("test");
        healthBarPanel.transform.localScale = new Vector3(eye.healthPercentage, 1, 1);
    }

}
