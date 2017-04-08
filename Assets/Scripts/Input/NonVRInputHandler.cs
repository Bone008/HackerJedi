using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonVRInputHandler : MonoBehaviour
{

    public HackerPlayer player;
    public Camera masterCamera;
    public Canvas masterHUD;

    public float rotationSpeed = 180;

    private Camera hackerCamera;

    private int[][] selectionPositions =
    {
        new int[]{ 0, 1 },
        new int[]{ 1, 0 },
        new int[]{ 0, -1 },
        new int[]{ -1, 0 },
    };
    private int selectedWeaponIndex = 0;

    private enum Perspective
    {
        hacker = 0,
        both = 1,
        master = 2
    };
    private Perspective currentPerspective = Perspective.hacker;

    void Start()
    {
        // get own camera
        hackerCamera = GetComponent<Camera>();

        // show initial perspective
        ShowCharacterPerspective(currentPerspective);
    }

    void Update()
    {
        // switch perspectives
        bool? switchPerspective = Util.InputGetAxisDown("NonVR Switch Perspective");
        if (switchPerspective != null)
        {
            if (switchPerspective == true)
                ShowCharacterPerspective(currentPerspective.Next());
            else
                ShowCharacterPerspective(currentPerspective.Previous());
        }
        
        // rotate hacker camera
        if (currentPerspective == Perspective.hacker || currentPerspective == Perspective.both)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            var angles = transform.localEulerAngles;
            angles.y += h * rotationSpeed * Time.deltaTime;
            angles.x -= v * rotationSpeed * Time.deltaTime;
            transform.localEulerAngles = angles;
        }
        
        // trigger input
        if (Input.GetButtonDown("Fire1"))
            player.SetTriggerDown(HackerHand.Left, true);
        if (Input.GetButtonUp("Fire1"))
            player.SetTriggerDown(HackerHand.Left, false);
        if (Input.GetButtonDown("Fire2"))
            player.SetTriggerDown(HackerHand.Right, true);
        if (Input.GetButtonUp("Fire2"))
            player.SetTriggerDown(HackerHand.Right, false);

        // switch weapon
        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine(SwitchWeapon());
        }
    }

    private IEnumerator SwitchWeapon()
    {
        selectedWeaponIndex = (selectedWeaponIndex + 1) % selectionPositions.Length;
        var touchPos = selectionPositions[selectedWeaponIndex];
        var touchPosVec = new Vector2(touchPos[0], touchPos[1]) * 0.8f;
        
        player.OpenAbilitySelectionWheel(HackerHand.Left);
        player.OpenAbilitySelectionWheel(HackerHand.Right);
        yield return new WaitForFixedUpdate();
        player.SetAbilitySelectionPosition(HackerHand.Left, touchPosVec);
        player.SetAbilitySelectionPosition(HackerHand.Right, touchPosVec);
        yield return new WaitForSeconds(0.2f);
        player.ConfirmAbilitySelection(HackerHand.Left, touchPosVec);
        player.ConfirmAbilitySelection(HackerHand.Right, touchPosVec);
        yield return new WaitForSeconds(0.1f);
        player.CloseAbilitySelectionWheel(HackerHand.Left);
        player.CloseAbilitySelectionWheel(HackerHand.Right);
    }

    private void ShowCharacterPerspective(Perspective perspective)
    {
        switch(perspective)
        {
            case Perspective.hacker:
                masterCamera.enabled = false;
                hackerCamera.enabled = true;
                SetCamWidth(hackerCamera, 1);
                masterHUD.enabled = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case Perspective.both:
                masterCamera.enabled = true;
                hackerCamera.enabled = true;
                SetCamWidth(masterCamera, 0.5f);
                SetCamWidth(hackerCamera, 0.5f, true);
                masterHUD.enabled = true;
                Cursor.lockState = CursorLockMode.None;
                break;

            case Perspective.master:
                masterCamera.enabled = true;
                hackerCamera.enabled = false;
                SetCamWidth(masterCamera, 1);
                masterHUD.enabled = true;
                Cursor.lockState = CursorLockMode.None;
                break;            
        }
        currentPerspective = perspective;
    }

    private void SetCamWidth(Camera cam, float newWidth, bool floatRight = false)
    {
        var rect = cam.rect;
        rect.width = newWidth;
        if (floatRight)
            rect.x = 1 - newWidth;
        else
            rect.x = 0;
        cam.rect = rect;
    }
}
