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
    public float handMovementSpeed = 1;
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

    private HackerHand currentHand = HackerHand.Left;

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

        // collect modifiers
        bool altHandControl = (Input.GetAxis("NonVR Modify Hand Control") > 0);
        bool altHandDirection = (Input.GetAxis("NonVR Modify Hand Direction") > 0);

        // rotate hacker camera
        if (!altHandControl && (currentPerspective == Perspective.Hacker || currentPerspective == Perspective.Both))
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

        // switch hacker hands
        bool? nextHand = Util.InputGetAxisDown("NonVR Switch Active Hand");
        if (nextHand == true)
            currentHand = HackerHand.Right;
        if (nextHand == false)
            currentHand = HackerHand.Left;

        // move hacker hands left/right/up/down
        if (altHandControl && (currentPerspective == Perspective.Hacker || currentPerspective == Perspective.Both))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            GameObject handGO = player.GetHandGO(currentHand);
            float oldDist = Vector3.Distance(handGO.transform.position, hackerCamera.transform.position);

            // move
            Vector3 newPosition = handGO.transform.position;
            Vector3 baseDirection = altHandDirection ? transform.forward : transform.up;
            newPosition += handMovementSpeed * baseDirection * Time.deltaTime * y;
            newPosition += handMovementSpeed * handGO.transform.right * Time.deltaTime * x;
            // only store, if weapon is still in sight
            Vector3 viewportPoint = hackerCamera.WorldToViewportPoint(newPosition);
            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1)
                handGO.transform.position = newPosition;

            // rotate away from camera to be able to aim
            handGO.transform.rotation = Quaternion.LookRotation(handGO.transform.position - hackerCamera.transform.position);
            handGO.transform.Rotate(-10, 0, 0); // to see shot lasers

            // force hand to stay in same distance
            if (!altHandDirection)
            {
                Vector3 direction = handGO.transform.position - hackerCamera.transform.position;
                Vector3 directionNorm = direction.normalized;
                directionNorm *= (oldDist - direction.magnitude);
                handGO.transform.position += directionNorm;
            }
        }

        // trigger input
        if (currentPerspective == Perspective.Hacker || currentPerspective == Perspective.Both)
        {
            if (Input.GetButtonDown("Fire1"))
                player.SetTriggerDown(HackerHand.Left, true);
            if (Input.GetButtonUp("Fire1"))
                player.SetTriggerDown(HackerHand.Left, false);
            if (Input.GetButtonDown("Fire2"))
                player.SetTriggerDown(HackerHand.Right, true);
            if (Input.GetButtonUp("Fire2"))
                player.SetTriggerDown(HackerHand.Right, false);
        }

        // switch weapon
        if (currentPerspective == Perspective.Hacker || currentPerspective == Perspective.Both)
        {
            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(SwitchWeapon(currentHand));
            }
        }
    }

    private IEnumerator SwitchWeapon(HackerHand hand)
    {
        selectedWeaponIndex = (selectedWeaponIndex + 1) % selectionPositions.Length;
        var touchPos = selectionPositions[selectedWeaponIndex];
        var touchPosVec = new Vector2(touchPos[0], touchPos[1]) * 0.8f;
        
        player.OpenAbilitySelectionWheel(hand);
        yield return new WaitForFixedUpdate();
        player.SetAbilitySelectionPosition(hand, touchPosVec);
        yield return new WaitForSeconds(0.2f);
        player.ConfirmAbilitySelection(hand, touchPosVec);
        yield return new WaitForSeconds(0.1f);
        player.CloseAbilitySelectionWheel(hand);
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
