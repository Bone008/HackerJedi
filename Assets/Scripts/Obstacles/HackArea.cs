using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackArea : ObstacleBase {

    public float timeToHack = 5.0f;
    public Color defaultColor = Color.green;
    public Color hackingColor = Color.red;
    public Color hackedColor = Color.yellow;

    private float timeRemaining;
    private GameObject hackerController;
    private Renderer hackAreaRenderer;

	// Use this for initialization
	void Start () {
        hackAreaRenderer = GetComponent<Renderer>();
        hackAreaRenderer.material.color = defaultColor;
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
            Destroy(gameObject.transform.parent.parent.gameObject);
            return;
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
            hackAreaRenderer.material.color = hackingColor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        var hackerHand = other.gameObject.GetGoInParentWithTag("HackerHand");
        
        // test if current hand left the hacking area
        if (hackerHand != null && hackerHand.Equals(hackerController))
        {
            hackerController = null;
            hackAreaRenderer.material.color = defaultColor;
        }
    }

}
