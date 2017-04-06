using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackArea : MonoBehaviour {

    public float timeToHack = 5.0f;
    public Color defaultColor = Color.green;
    public Color hackingColor = Color.red;
    public Color hackedColor = Color.yellow;

    private float timeRemaining;
    private SteamVR_TrackedController hackerController;
    private Renderer hackAreaRenderer;

	// Use this for initialization
	void Start () {
        hackAreaRenderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hackerController != null)
        {
            timeRemaining -= Time.deltaTime;
        }

        if(timeRemaining < 0)
        {
            hackAreaRenderer.material.color = hackedColor;

            // TODO cool animation
            Destroy(gameObject.transform.parent.gameObject);
            return;
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        var other = collider.gameObject.GetComponentInParent<SteamVR_TrackedController>();
        if (other != null)
        {
            timeRemaining = timeToHack;
            hackerController = other;
            hackAreaRenderer.material.color = hackingColor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.GetComponentInParent<SteamVR_TrackedController>() == hackerController)
        {
            hackerController = null;
            hackAreaRenderer.material.color = defaultColor;
        }
    }

}
