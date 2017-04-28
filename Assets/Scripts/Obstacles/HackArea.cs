using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackArea : MonoBehaviour {

    public float timeToHack = 5.0f;
    public Color defaultColor = Color.green;
    public Color hackingColor = Color.red;
    private Color targetColor;
    public float colorTransitionMultiplier;

    public Animator noiceAnimator;
    
    private float timeRemaining;
    private GameObject hackerController;
    private Renderer hackAreaRenderer;

    private Coroutine deathCoroutine;

	void Start () {
        // get renderer and set default color
        hackAreaRenderer = GetComponent<Renderer>();
        targetColor = defaultColor;
	}
	
	void Update () {
        // show hack area when animation has finished
        if(!hackAreaRenderer.enabled)
        {
            if (noiceAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || noiceAnimator.IsInTransition(0))
                return;

            hackAreaRenderer.enabled = true;
        }

        // decrease hacking counter
		if(hackerController != null)
            timeRemaining -= Time.deltaTime;

        // interpolate color
        hackAreaRenderer.material.color = Color.Lerp(hackAreaRenderer.material.color, targetColor, Time.deltaTime * colorTransitionMultiplier);

        // hackarea deaded
        if (timeRemaining < 0)
        {
            targetColor = hackingColor;
            targetColor.a = 0;

            if (deathCoroutine == null)
                deathCoroutine = StartCoroutine(Deathded());
        }        
    }

    void OnTriggerEnter(Collider collider)
    {
        // already a hand inside
        if (hackerController != null)
            return;

        // get hand go
        var hackerHand = collider.gameObject.GetGoInParentWithTag("HackerHand");

        if (hackerHand != null)
        {
            timeRemaining = timeToHack;
            hackerController = hackerHand;
            targetColor = hackingColor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        var hackerHand = other.gameObject.GetGoInParentWithTag("HackerHand");
        
        // test if current hand left the hacking area
        if (hackerHand != null && hackerHand.Equals(hackerController))
        {
            hackerController = null;
            targetColor = defaultColor;
        }
    }

    // this should be set as callback in BlockPlatform script
    public void OnPlatformBlocked()
    {
        // start noice animation
        noiceAnimator.enabled = true;
        hackAreaRenderer.enabled = true;
    }

    private IEnumerator Deathded()
    {
        // start animation
        noiceAnimator.Play("Reverse");

        // wait for animation end
        yield return new WaitWhile(() => noiceAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || noiceAnimator.IsInTransition(0));

        // move terminal into ground
        Vector3 oldPosition = transform.parent.position;
        Vector3 targetPosition = transform.parent.position - new Vector3(0, 9, 0);
        float progress = 0;

        while (transform.parent.position != targetPosition)
        {
            transform.parent.position = Vector3.Slerp(oldPosition, targetPosition, progress);
            progress += Time.deltaTime / 4;
            yield return 0;
        }

        // destroy gameobject to free platform
        Destroy(gameObject.transform.parent.gameObject);
    }

}
