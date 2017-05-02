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

        yield return this.AnimateVector(gateCloseDuration, gateOpenPos, gateClosedPos, gateCloseCurve.Evaluate, p => gate.localPosition = p);

        if (finishCallback != null)
            finishCallback();
    }
}
