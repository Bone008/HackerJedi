using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockPlatform : MonoBehaviour {

    public UnityEvent onPlatformBlocked;
    private Platform blockedPlatform;
    private bool blocked;
    
    void OnTriggerEnter(Collider other)
    {
        Platform platform = other.GetComponentInParent<Platform>();
        if (platform == null)
            return;

        blockedPlatform = platform;
        blocked = true;
        if (onPlatformBlocked != null)
            onPlatformBlocked.Invoke();
    }
    
    void Update()
    {
        if (blocked)
            blockedPlatform.DisableForSec(0.5f);
    }
}
