using System.Collections;
using UnityEngine;

public class Turret : EnemyBase
{
    public float turretActiveSec = 5.0f;

    // turret game objects
    public Transform barrel;
    public Transform turretCamTarget;

    // master game objects
    private GameObject masterGO;
    private Canvas masterHUD;
    private Camera masterCam;

    // modes
    private enum Mode
    {
        MoveCamToTurret,
        InTurret,
        MoveCamToEye
    };
    private Mode currentMode = Mode.MoveCamToTurret;

    // cam movement
    public float camTransitionTime;
    private float camMovementProgress = 0;
    private float camRotationProgress = 0;
    private Vector3 initialMasterCamPos;
    private Quaternion initialMasterCamRot;
    private Transform initialMasterCamParent;

    void Start()
    {
        // get gameobjects
        masterGO = GameObject.FindGameObjectWithTag("Master");
        masterHUD = masterGO.GetComponentInChildren<Canvas>();
        masterCam = masterGO.GetComponentInChildren<Camera>();

        // store initial position of master cam for later
        initialMasterCamPos = masterCam.transform.position;
        initialMasterCamRot = masterCam.transform.rotation;

        // stop master interaction during animation
        SwitchMasterScripts(false, false);
    }

    void Update()
    {
        switch (currentMode)
        {
            case Mode.MoveCamToTurret:
                // move and rotate to turret
                camMovementProgress += Time.deltaTime;
                camRotationProgress += Time.deltaTime;
                masterCam.transform.position = Vector3.Slerp(initialMasterCamPos, turretCamTarget.position, camMovementProgress / camTransitionTime);
                masterCam.transform.rotation = Quaternion.Slerp(initialMasterCamRot, turretCamTarget.rotation, camRotationProgress / camTransitionTime);

                if (camRotationProgress >= camTransitionTime)
                {
                    // store initial parent of cam for later, set barrel as parent
                    initialMasterCamParent = masterCam.transform.parent;
                    masterCam.transform.SetParent(turretCamTarget);

                    // switch to turret-master
                    SwitchMasterScripts(false, true);

                    // disable master again
                    StartCoroutine(DisableMasterAgain());

                    currentMode = Mode.InTurret;
                }

                break;

            case Mode.InTurret:
                break; // TurretMaster handles this

            case Mode.MoveCamToEye:
                // move and rotate to eye
                camMovementProgress += Time.deltaTime;
                camRotationProgress += Time.deltaTime;
                masterCam.transform.position = Vector3.Slerp(turretCamTarget.position, initialMasterCamPos, camMovementProgress / camTransitionTime);
                masterCam.transform.rotation = Quaternion.Slerp(turretCamTarget.rotation, initialMasterCamRot, camRotationProgress / camTransitionTime);

                if (camRotationProgress >= camTransitionTime)
                {
                    // restore initial parent
                    masterCam.transform.SetParent(initialMasterCamParent);

                    // switch back to default master
                    SwitchMasterScripts(true, false);

                    // destroy turret
                    Destroy(gameObject);
                    return;
                }

                break;
        }
    }

    private void SwitchMasterScripts(bool normal, bool turret)
    {
        // default script
        masterGO.GetComponent<Master>().enabled = normal;

        // turret script
        masterGO.GetComponent<TurretMaster>().turretBarrel = barrel;
        masterGO.GetComponent<TurretMaster>().enabled = turret;

        // disable hud and lock cursor
        masterHUD.enabled = normal;
        Cursor.lockState = turret ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private IEnumerator DisableMasterAgain()
    {
        yield return new WaitForSeconds(turretActiveSec);
        SwitchMasterScripts(false, false);
        camMovementProgress = 0;
        camRotationProgress = 0;
        currentMode = Mode.MoveCamToEye;
    }

}
