using System.Collections;
using UnityEngine;

public class ShootPlayer : MonoBehaviour {

    public Gun gun;
    public float hitRange;
    public float fireDelay;

    private Transform target;
    private Coroutine firingCoroutine;
    
	void Start ()
    {
        // locate player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        
        // make sure gun fires on the correct layer so it hits the hacker
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

    private void OnDisable()
    {
        StopCoroutine(firingCoroutine);
        firingCoroutine = null;
    }

}
