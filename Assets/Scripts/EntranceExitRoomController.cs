using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceExitRoomController : MonoBehaviour {

    public Transform gate;
    public float gateCloseDuration = 0.5f;
    public AnimationCurve gateCloseCurve;

    private Vector3 gateClosedPos, gateOpenPos;

	void Start () {
        gateClosedPos = gate.localPosition;
        gateOpenPos = gate.localPosition + (Vector3.down * gate.GetComponent<Collider>().bounds.size.y);
        gate.gameObject.SetActive(false); // note that the GO has to be active for the collider bounds calculation to work
    }

    public void CloseGate(Action finishCallback = null)
    {
        StartCoroutine(_CloseGateAnimation(finishCallback));

        foreach (var particleSystem in GetComponentsInChildren<ParticleSystem>())
            particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }

    private IEnumerator _CloseGateAnimation(Action finishCallback)
    {
        gate.gameObject.SetActive(true);

        // animate position to closedPos
        float t = 0;
        while(t < gateCloseDuration)
        {
            gate.localPosition = Vector3.Lerp(gateOpenPos, gateClosedPos, gateCloseCurve.Evaluate(t / gateCloseDuration));
            yield return null;
            t += Time.deltaTime;
        }
        gate.localPosition = gateClosedPos;

        if (finishCallback != null)
            finishCallback();
    }
}
