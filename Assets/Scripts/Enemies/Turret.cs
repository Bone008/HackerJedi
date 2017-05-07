using System.Collections;
using UnityEngine;

public class Turret : EnemyBase
{
    private GameObject masterGO;
    private Canvas masterHUD;
    private Transform masterCam;
    private AudioSource audio;

    [Header("Eye transition")]
    public float eyeTransitionTime;
    public Transform eyeTarget;
    private Transform eye;
    private Animator animator;

    [Header("Timescale transition")]
    public float timeScaleTransitionTime;
    public float timeScaleGoal = 0.4f;

    [Header("In Turret")]
    public float turretActiveSec = 5.0f;
    public Transform barrel;

    void Start()
    {
        // get all the gameobjects
        masterGO = GameObject.FindGameObjectWithTag("Master");
        masterHUD = masterGO.GetComponentInChildren<Canvas>();
        Debug.Log(masterHUD);
        masterCam = masterGO.GetComponentInChildren<Camera>().transform;
        eye = GameObject.FindGameObjectWithTag("MasterEye").transform;
        animator = GetComponent<Animator>();
        audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();

        // stop master interaction during animation
        SwitchMasterScripts(false, false);

        // start time scale before movement finished
        this.Delayed(eyeTransitionTime - timeScaleTransitionTime,
            () => this.Animate(timeScaleTransitionTime, p =>
            {
                Time.timeScale = Mathf.Lerp(1.0f, timeScaleGoal, p);
                audio.pitch = Time.timeScale;
            }, true));

        // start everything
        StartCoroutine(DoTurretStuff());
    }
    
    private void SwitchMasterScripts(bool normal, bool turret)
    {
        // default script
        masterGO.GetComponent<Master>().enabled = normal;

        // turret script
        masterGO.GetComponent<TurretMaster>().turret = transform;
        masterGO.GetComponent<TurretMaster>().turretBarrel = barrel;
        masterGO.GetComponent<TurretMaster>().enabled = turret;

        // disable hud and lock cursor
        masterHUD.enabled = normal;
        Cursor.lockState = turret ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private IEnumerator DoTurretStuff()
    {
        // set cam parent to eye
        eye.localRotation = masterCam.localRotation;
        Transform initialMasterCamParent = masterCam.transform.parent;
        masterCam.transform.SetParent(eye);

        // move to turret
        Vector3 initialEyePos = eye.position;
        Quaternion initialEyeRot = eye.rotation;
        Vector3 initialEyeScale = eye.localScale;
        yield return this.Animate(eyeTransitionTime, progress =>
        {
            eye.position = Vector3.Lerp(initialEyePos, eyeTarget.position, progress);
            eye.rotation = Quaternion.Lerp(initialEyeRot, eyeTarget.rotation, progress);
            eye.localScale = Vector3.Lerp(initialEyeScale, eyeTarget.localScale, progress);
        }, true);

        // set eye parent
        Transform initialEyeParent = eye.transform.parent;
        eye.transform.SetParent(transform);        

        // switch to turret-master
        SwitchMasterScripts(false, true);

        // disable scripts of eye
        foreach (var script in eye.GetComponents<MonoBehaviour>())
            script.enabled = false;

        // in turret
        yield return new WaitForSecondsRealtime(turretActiveSec);

        // disable any interaction
        SwitchMasterScripts(false, false);

        // move back again
        Quaternion midEyeRot = eye.rotation;
        this.Animate(eyeTransitionTime, progress =>
        {
            eye.position = Vector3.Lerp(eyeTarget.position, initialEyePos, progress);
            eye.rotation = Quaternion.Lerp(midEyeRot, initialEyeRot, progress);
            eye.localScale = Vector3.Lerp(eyeTarget.localScale, initialEyeScale, progress);
        }, true);

        // restore timescale
        yield return new WaitForSecondsRealtime(eyeTransitionTime - timeScaleTransitionTime);
        yield return this.Animate(timeScaleTransitionTime, p =>
        {
            Time.timeScale = Mathf.Lerp(timeScaleGoal, 1.0f, p);
            audio.pitch = Time.timeScale;
        }, true);

        // restore initial parent
        eye.transform.SetParent(initialEyeParent);
        masterCam.transform.SetParent(initialMasterCamParent);

        // switch back to default master
        SwitchMasterScripts(true, false);

        // re-enable scripts of eye again
        foreach (var script in eye.GetComponents<MonoBehaviour>())
            script.enabled = true;

        // destroy turret
        Destroy(gameObject);
    }

}
