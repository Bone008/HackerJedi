using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockPlatform : MonoBehaviour {

    public UnityEvent onPlatformBlocked;
    private Platform blocked;
    
    void OnTriggerEnter(Collider other)
    {
        Platform platform = other.GetComponentInParent<Platform>();
        if (platform != null)
        {
            platform.running = false;
            blocked = platform;
            if (onPlatformBlocked != null)
                onPlatformBlocked.Invoke();
        }
    }

    void OnDestroy()
    {
        if(blocked != null)
            blocked.running = true;
    }
}
