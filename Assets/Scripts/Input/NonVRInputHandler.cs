using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonVRInputHandler : MonoBehaviour
{

    public HackerPlayer player;
    public Camera masterCamera;

    public float rotationSpeed = 180;

    private int[][] selectionPositions =
    {
        new int[]{ 0, 1 },
        new int[]{ 1, 0 },
        new int[]{ 0, -1 },
        new int[]{ -1, 0 },
    };
    private int selectedWeaponIndex = 0;

    void Start()
    {
        var rect = masterCamera.rect;
        rect.width = 0.5f;
        masterCamera.rect = rect;
    }

    void Update()
    {
        // rotate camera
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        var angles = transform.localEulerAngles;
        angles.y += h * rotationSpeed * Time.deltaTime;
        angles.x -= v * rotationSpeed * Time.deltaTime;
        transform.localEulerAngles = angles;

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
}
