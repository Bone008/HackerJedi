using System.Collections;
using UnityEngine;

public class ShootPlayer : MonoBehaviour {

    public float hitRange;
    public float fireDelay;

    private Transform target;
    private Gun gun;
    private Coroutine firingCoroutine;
    
	void Start ()
    {
        // locate player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        
        // get gun component from children
        gun = GetComponentInChildren<Gun>();
        gun.layer = LayerMask.NameToLayer("Hacker");
    }
	
	void Update ()
    {
        // fire while in range
        if ((target.position - transform.position).sqrMagnitude <= hitRange * hitRange)
        {
            if (firingCoroutine == null)
                firingCoroutine = StartCoroutine(FireWhenReady());
        }
        else if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    private IEnumerator FireWhenReady()
    {
        while (true)
        {
            gun.FireOnce();
            yield return new WaitForSeconds(fireDelay);
        }
    }
}
