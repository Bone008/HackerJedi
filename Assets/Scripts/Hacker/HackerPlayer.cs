using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum HackerHand { Left = 0, Right = 1 }

public class HackerPlayer : MonoBehaviour
{
    public GameObject fullHacker;

    public GameObject[] handGameObjects = new GameObject[2];

    public GameObject abilitySelectionPrefab;
    public AbilityType initialAbilityLeft;
    public AbilityType initialAbilityRight;

    [Header("UI")]
    public GameObject deathScreenElement;

    // one entry per hand
    private Dictionary<AbilityType, GameObject>[] allAbilityGOs = { new Dictionary<AbilityType, GameObject>(), new Dictionary<AbilityType, GameObject>() };
    private AbilityType[] equippedAbilities = { AbilityType.None, AbilityType.None };
    private AbilitySelectionWheel[] selectionWheels = { null, null };

    // one entry for both hands
    private Dictionary<AbilityType, GameObject> allUltimateGOs = new Dictionary<AbilityType, GameObject>();
    private AbstractUltimate activeUltimate = null; // the ultimate script that is currently active, or null if none are

    // after death
    public Texture viewBlocker;
    private Coroutine afterDeathCoroutine = null;
    private float fadeOutPercentage = 0;

    private void Start()
    {
        Debug.Assert(handGameObjects.Length == 2);

        SpawnAbilityInstances();
        SpawnUltimateInstances();
        EquipAbility(HackerHand.Left, initialAbilityLeft);
        EquipAbility(HackerHand.Right, initialAbilityRight);
        
        deathScreenElement.SetActive(false);
    }


    public GameObject GetHandGO(HackerHand hand)
    {
        return handGameObjects[(int)hand];
        //return (hand == HackerHand.Left ? leftHand : rightHand);
    }

    private GameObject GetEquippedAbilityGO(HackerHand hand)
    {
        GameObject go;
        if (allAbilityGOs[(int)hand].TryGetValue(equippedAbilities[(int)hand], out go))
            return go;
        else
            return null;
    }

    private AbstractAbility GetEquippedAbilityScript(HackerHand hand)
    {
        GameObject go = GetEquippedAbilityGO(hand);
        if (go != null)
            return go.GetComponent<AbstractAbility>(); // may also return null if the ability has no script (yet)
        else
            return null;
    }


    // instantiates all ability GOs (disabled) for each hand and stores them in allAbilityGOs
    private void SpawnAbilityInstances()
    {
        // for both hands
        for (int i = 0; i < 2; i++)
        {
            HackerHand hand = (HackerHand)i;

            var abilityPrefabs = abilitySelectionPrefab.GetComponent<AbilitySelectionWheel>().abilityPrefabs;
            foreach (var prefab in abilityPrefabs)
            {
                var go = Instantiate(prefab, GetHandGO(hand).transform);
                go.SetActive(false);

                var abilityScript = go.GetComponent<AbstractAbility>();
                abilityScript.InitHackerPlayer(transform);

                if (abilityScript.needsMirroring && hand == HackerHand.Left)
                    // mirror along X axis
                    go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);

                allAbilityGOs[i].Add(abilityScript.Type, go);
            }
        }
    }

    // instantiates ultimate GOs (disabled) and stores them in allUltimateGOs
    private void SpawnUltimateInstances()
    {
        var ultimateRoot = new GameObject("Ultimates");
        ultimateRoot.transform.SetParent(transform);

        var ultimatePrefabs = abilitySelectionPrefab.GetComponent<AbilitySelectionWheel>().ultimatePrefabs;
        foreach (var prefab in ultimatePrefabs)
        {
            var go = Instantiate(prefab, Vector3.zero, Quaternion.identity, ultimateRoot.transform);
            go.SetActive(false);

            var ult = go.GetComponent<AbstractUltimate>();
            ult.InitHackerPlayer(transform);
            ult.InitHands(GetHandGO(HackerHand.Left).transform, GetHandGO(HackerHand.Right).transform);

            allUltimateGOs.Add(ult.Type, go);
        }
    }



    private void EquipAbility(HackerHand hand, AbilityType type)
    {
        int i = (int)hand;
        Debug.Assert(0 <= i && i <= 1);

        if (!allAbilityGOs[i].ContainsKey(type))
        {
            Debug.LogWarning("cannot equip ability type, no prefab configured: " + type);
            return;
        }


        // disable old equipment
        if (allAbilityGOs[i].ContainsKey(equippedAbilities[i]))
        {
            var script = GetEquippedAbilityScript(hand);
            if (script != null)
            {
                // make sure that we stop firing before disabling
                script.SetTriggerDown(false);
                script.SetGripDown(false);
            }
            allAbilityGOs[i][equippedAbilities[i]].SetActive(false);
        }

        // enable new equipment
        allAbilityGOs[i][type].SetActive(true);
        equippedAbilities[i] = type;

        UpdateActiveUltimate();
    }


    // needs to be called whenever a prerequisite for ultimate activation changes (equipped ability, unlocked abilities)
    public void UpdateActiveUltimate()
    {
        activeUltimate = null;

        foreach (var kvp in allUltimateGOs)
        {
            AbilityType ability = kvp.Key;
            GameObject ultimateGO = kvp.Value;
            var ultimate = ultimateGO.GetComponent<AbstractUltimate>();

            bool active = equippedAbilities[0] == ability && equippedAbilities[1] == ability
                            && HackerProgression.Instance.IsUltimateUnlocked(ability);

            if (!active && ultimateGO.activeSelf)
            {
                // make sure to stop triggering
                ultimate.SetTriggerDown(false);
                ultimate.SetGripDown(false);
            }
            else if(active)
            {
                activeUltimate = ultimate;
            }

            ultimateGO.SetActive(active);
        }
    }


    // called by the concrete input handler when the trigger state has changed
    public void SetTriggerDown(HackerHand hand, bool state)
    {
        var abilityScript = GetEquippedAbilityScript(hand);
        if (abilityScript != null)
            abilityScript.SetTriggerDown(state);

        if(activeUltimate != null)
        {
            var otherScript = GetEquippedAbilityScript(hand.Next());
            bool both = (abilityScript != null && otherScript != null && abilityScript.IsTriggerDown && otherScript.IsTriggerDown);
            activeUltimate.SetTriggerDown(both);
        }
    }

    // called by the concrete input handler when the grip state has changed
    public void SetGripDown(HackerHand hand, bool state)
    {
        var abilityScript = GetEquippedAbilityScript(hand);
        if (abilityScript != null)
            abilityScript.SetGripDown(state);

        if (activeUltimate != null)
        {
            var otherScript = GetEquippedAbilityScript(hand.Next());
            bool both = (abilityScript != null && otherScript != null && abilityScript.IsGripDown && otherScript.IsGripDown);
            activeUltimate.SetGripDown(both);
        }
    }


    public void OpenAbilitySelectionWheel(HackerHand hand)
    {
        if (selectionWheels[(int)hand] != null)
            return; // already open

        var go = Instantiate(abilitySelectionPrefab);
        go.transform.SetParent(GetHandGO(hand).transform, false);
        selectionWheels[(int)hand] = go.GetComponent<AbilitySelectionWheel>();
    }

    public void CloseAbilitySelectionWheel(HackerHand hand)
    {
        AbilitySelectionWheel wheel = selectionWheels[(int)hand];

        if (wheel == null)
            return; // already closed

        Destroy(wheel.gameObject);
        selectionWheels[(int)hand] = null;
    }

    public void SetAbilitySelectionPosition(HackerHand hand, Vector2 position)
    {
        AbilitySelectionWheel wheel = selectionWheels[(int)hand];
        if (wheel == null)
            return; // not open

        wheel.SetPreviewPosition(position);
    }

    public void ConfirmAbilitySelection(HackerHand hand, Vector2 position)
    {
        StartCoroutine(_ConfirmCoroutineHelper(hand, position));
    }

    private IEnumerator _ConfirmCoroutineHelper(HackerHand hand, Vector2 position)
    {
        yield return null; // delay by one frame

        AbilitySelectionWheel wheel = selectionWheels[(int)hand];
        if (wheel == null)
            yield break; // not open

        AbilityType? newAbility = wheel.ConfirmSelection(position);
        if(newAbility != null)
        {
            EquipAbility(hand, newAbility.Value);
            //CloseAbilitySelectionWheel(hand);
        }
    }

    public void OnDeath(HealthResource health)
    {
        //Debug.Log("You deaded!");
        //health.RestoreFullHealth();

        //deathScreenElement.SetActive(true);
        //this.Delayed(2.0f, () => deathScreenElement.SetActive(false));

        if (afterDeathCoroutine == null)
            afterDeathCoroutine = StartCoroutine(AfterDeadedCoroutine());
    }

    private void OnGUI()
    {
        // fade out on death
        if (afterDeathCoroutine != null)
        {
            GUI.color = new Color(0, 0, 0, fadeOutPercentage);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), viewBlocker); // TODO i hope this also works with vive...
        }
    }
    private IEnumerator AfterDeadedCoroutine()
    {
        // disable gui
        foreach (Canvas c in fullHacker.GetComponentsInChildren<Canvas>())
            c.enabled = false;

        // move player above the map
        Vector3 startPos = fullHacker.transform.position;
        Vector3 goalPos = new Vector3(0, 50, 0);

        while (fadeOutPercentage < 1.0f)
        {
            fadeOutPercentage += Time.deltaTime * 0.5f;
            fullHacker.transform.position = Vector3.Slerp(startPos, goalPos, fadeOutPercentage);
            yield return 0;
        }

        // load lose_screen scene
        SceneManager.LoadScene(4);
    }

}
