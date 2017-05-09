using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackArea : MonoBehaviour {

    public float timeToHack = 5.0f;
    public Color defaultColor = Color.green;
    public Color hackingColor = Color.red;
    private Color targetColor;
    public float colorTransitionMultiplier;
    public AudioClip finish;
    public AudioClip cancel;

    public Animator noiceAnimator;

    [Header("Progress UI")]
    public Text progressStatusText;
    public RectTransform progressBar;

    private float timeRemaining;
    private GameObject hackerController;
    private Renderer hackAreaRenderer;

    private Coroutine deathCoroutine;

	void Start () {
        // get renderer and set default color
        hackAreaRenderer = GetComponent<Renderer>();
        targetColor = defaultColor;

        timeRemaining = timeToHack;
        progressStatusText.text = "Hack me!";
        UpdateProgress();
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
        if (hackerController != null)
        {
            timeRemaining -= Time.deltaTime;
            UpdateProgress();
        }

        // interpolate color
        hackAreaRenderer.material.color = Color.Lerp(hackAreaRenderer.material.color, targetColor, Time.deltaTime * colorTransitionMultiplier);

        // hackarea deaded
        if (timeRemaining < 0)
        {
            targetColor = hackingColor;
            targetColor.a = 0;
            progressStatusText.text = "ACCESS GRANTED";
            

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

            progressStatusText.text = "Hacking in progress ...";
            UpdateProgress();
        }
    }

    void OnTriggerExit(Collider other)
    {
        var hackerHand = other.gameObject.GetGoInParentWithTag("HackerHand");
        
        // test if current hand left the hacking area
        if (hackerHand != null && hackerHand.Equals(hackerController) && timeRemaining > 0)
        {
            hackerController = null;
            targetColor = defaultColor;
            timeRemaining = timeToHack;
            AudioSource.PlayClipAtPoint(cancel, this.transform.position, 0.8f);
            progressStatusText.text = "Hack me!";
            UpdateProgress();
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
        AudioSource.PlayClipAtPoint(finish, this.transform.position, 0.8f);
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
            transform.parent.position = Vector3.Lerp(oldPosition, targetPosition, progress * progress);
            progress += Time.deltaTime / 3;
            yield return 0;
        }

        // destroy gameobject to free platform
        Destroy(gameObject.transform.parent.gameObject);
    }


    private void UpdateProgress()
    {
        Vector3 s = progressBar.localScale;
        s.x = 1.0f - Mathf.Clamp01(timeRemaining / timeToHack);
        progressBar.localScale = s;
    }

}
