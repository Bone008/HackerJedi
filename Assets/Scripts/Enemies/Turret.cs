using System.Collections;
using UnityEngine;

public class Turret : EnemyBase
{
    private GameObject masterGO;
    private Canvas masterHUD;
    private Transform masterCam;
    private AudioSource audio;
    public AudioClip turretArrival;
    private GameObject platform;

    [Header("Eye transition")]
    public float eyeTransitionTime;
    public Transform eyeTarget;
    public Transform camTarget;
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
        platform = GameObject.FindGameObjectWithTag("Platform");

        // stop master interaction during animation
        SwitchMasterScripts(false, false);
        //AudioSource.PlayClipAtPoint(turretArrival, this.transform.position, 1f);
        if (turretArrival != null)
        {
            var go = new GameObject("Turret Arrival");
            go.transform.position = this.transform.position;
            var audio = go.AddComponent<AudioSource>();
            audio.clip = turretArrival;
            audio.volume = 1f;
            audio.Play();
            this.Delayed(turretArrival.length + 0.5f, () => Destroy(go));
        }
        // start time scale before movement finished
        this.Delayed(eyeTransitionTime - timeScaleTransitionTime,
            () => this.Animate(timeScaleTransitionTime, p =>
            {
                Time.timeScale = Mathf.Lerp(1.0f, timeScaleGoal, p);
                audio.pitch = Time.timeScale;
            }, true));

        // rotate towards player
        transform.LookAt(platform.transform, new Vector3(0, 1, 0));

        // start everything
        StartCoroutine(DoTurretStuff());
    }

    private void OnDestroy()
    {
        // reset time scale
        Time.timeScale = 1;
        audio.pitch = 1;
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
        //eye.localRotation = masterCam.localRotation;
        //Transform initialMasterCamParent = masterCam.transform.parent;
        //masterCam.transform.SetParent(eye);

        // move to turret
        Vector3 initialEyePos = eye.position;
        Quaternion initialEyeRot = eye.rotation;
        Vector3 initialEyeScale = eye.localScale;
        Vector3 initialCamPos = masterCam.position;
        Quaternion initialCamRot = masterCam.rotation;
        yield return this.Animate(eyeTransitionTime, progress =>
        {
            eye.position = Vector3.Lerp(initialEyePos, eyeTarget.position, progress);
            eye.rotation = Quaternion.Lerp(initialEyeRot, eyeTarget.rotation, progress);
            eye.localScale = Vector3.Lerp(initialEyeScale, eyeTarget.localScale, progress);
            masterCam.position = Vector3.Lerp(initialCamPos, camTarget.position, progress);
            masterCam.rotation = Quaternion.Lerp(initialCamRot, camTarget.rotation, progress);
        }, true);

        // set eye and cam parent
        Transform initialEyeParent = eye.transform.parent;
        eye.transform.SetParent(transform, true);
        Transform initialMasterCamParent = masterCam.transform.parent;
        masterCam.transform.SetParent(barrel, true);
        masterCam.localRotation = Quaternion.Euler(0, -180, 0); // dont ask

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
        Quaternion midCamRot = masterCam.rotation;
        this.Animate(eyeTransitionTime, progress =>
        {
            eye.position = Vector3.Lerp(eyeTarget.position, initialEyePos, progress);
            eye.rotation = Quaternion.Lerp(midEyeRot, initialEyeRot, progress);
            eye.localScale = Vector3.Lerp(eyeTarget.localScale, initialEyeScale, progress);
            masterCam.position = Vector3.Lerp(camTarget.position, initialCamPos, progress);
            masterCam.rotation = Quaternion.Lerp(midCamRot, initialCamRot, progress);
        }, true);

        // restore timescale
        yield return new WaitForSecondsRealtime(eyeTransitionTime - timeScaleTransitionTime);
        yield return this.Animate(timeScaleTransitionTime, p =>
        {
            Time.timeScale = Mathf.Lerp(timeScaleGoal, 1.0f, p);
            audio.pitch = Time.timeScale;
        }, true);

        // restore initial parent
        eye.transform.SetParent(initialEyeParent, true);
        masterCam.transform.SetParent(initialMasterCamParent, true);

        // switch back to default master
        SwitchMasterScripts(true, false);

        // re-enable scripts of eye again
        foreach (var script in eye.GetComponents<MonoBehaviour>())
            script.enabled = true;

        // destroy turret
        Destroy(gameObject);
    }

}
