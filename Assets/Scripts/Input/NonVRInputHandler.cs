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
    public float movementSpeed = 5;
    public float blockSize = 3;

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
        Hacker = 0,
        Both = 1,
        Master = 2
    };
    private Perspective currentPerspective = Perspective.Hacker;

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
        if (currentPerspective == Perspective.Hacker || currentPerspective == Perspective.Both)
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            var angles = transform.localEulerAngles;
            angles.y += x * rotationSpeed * Time.deltaTime;
            angles.x -= y * rotationSpeed * Time.deltaTime;
            
            // prevent bottom-up camera
            if (angles.x < 200)
                angles.x = Mathf.Min(angles.x, 89); // 0-90
            else
                angles.x = Mathf.Max(angles.x, 271); // 270-360

            transform.localEulerAngles = angles;
        }

        // move hacker
        if (currentPerspective == Perspective.Hacker || currentPerspective == Perspective.Both)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            float hbs = blockSize / 2;

            Vector3 newPosition = transform.localPosition;
            newPosition += movementSpeed * transform.forward * Time.deltaTime * y;
            newPosition += movementSpeed * transform.right * Time.deltaTime * x;
            newPosition.x = Mathf.Clamp(newPosition.x, -hbs, hbs);
            newPosition.z = Mathf.Clamp(newPosition.z, -hbs, hbs);
            newPosition.y = transform.localPosition.y;
            transform.localPosition = newPosition;            
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
            case Perspective.Hacker:
                masterCamera.enabled = false;
                hackerCamera.enabled = true;
                SetCamWidth(hackerCamera, 1);
                masterHUD.enabled = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case Perspective.Both:
                masterCamera.enabled = true;
                hackerCamera.enabled = true;
                SetCamWidth(masterCamera, 0.5f);
                SetCamWidth(hackerCamera, 0.5f, true);
                masterHUD.enabled = true;
                Cursor.lockState = CursorLockMode.None;
                break;

            case Perspective.Master:
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
