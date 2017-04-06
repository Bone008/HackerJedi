using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlatform : MonoBehaviour {

    private Platform blocked;
    
    void OnTriggerEnter(Collider other)
    {
        Platform platform = other.GetComponentInParent<Platform>();
        if (platform != null)
        {
            platform.running = false;
            blocked = platform;
        }
    }

    void OnDestroy()
    {
        if(blocked != null)
            blocked.running = true;
    }
}
