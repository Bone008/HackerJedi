using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    [Header("MasterUI")]
    public Text textMaster;
    public GameObject masterUI;

    [Header("HackerUI_NonVR")]
    public Text textHacker;
    public GameObject hackerUI;

    [Header("HackerUI_VR")]
    public GameObject hackerUI_VR;
    public Text text1, text2, text3, text4;

    private bool transitioning = false;

    // Update is called once per frame
    void Update () {

        if (GameData.Instance.isPaused)
        {
            if (Input.GetKey("r"))
                hackerResumeGame();
            if (Input.GetKey(KeyCode.Escape))
                masterResumeGame();
            unpauseGame();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) || GameData.Instance.hackerIsReady)
        {
            pauseGame();
        }
    }

    public void masterResumeGame()
    {
        GameData.Instance.masterIsReady = true;
    }

    public void hackerResumeGame()
    {
        Debug.Log("Hacker ready");
        GameData.Instance.hackerIsReady = true;
    }

    void pauseGame()
    {
        if (transitioning)
            return;

        GameData.Instance.hackerIsReady = false;
        GameData.Instance.masterIsReady = false;
        transitioning = true;
        StartCoroutine(pauseWarning());
    }

    IEnumerator pauseWarning()
    {
        for (int i = 3; i >= 0; i--)
        {
            textMaster.text = textHacker.text = text1.text = text2.text = text3.text = text4.text = "Game is going to pause in:\n" + i;

            textMaster.CrossFadeAlpha(.75f, .45f, true);
            if (GameData.Instance.viveActive)
            {
                text1.CrossFadeAlpha(.75f, .45f, true);
                text2.CrossFadeAlpha(.75f, .45f, true);
                text3.CrossFadeAlpha(.75f, .45f, true);
                text4.CrossFadeAlpha(.75f, .45f, true);
            }
            else
                textHacker.CrossFadeAlpha(.75f, .45f, true);

            yield return new WaitForSeconds(.5f);

            textMaster.CrossFadeAlpha(1, .45f, true);
            if (GameData.Instance.viveActive)
            {
                text1.CrossFadeAlpha(1f, .45f, true);
                text2.CrossFadeAlpha(1f, .45f, true);
                text3.CrossFadeAlpha(1f, .45f, true);
                text4.CrossFadeAlpha(1f, .45f, true);
            }
            else
                textHacker.CrossFadeAlpha(1f, .45f, true);

            yield return new WaitForSeconds(.5f);
        }
        textMaster.CrossFadeAlpha(0, 1, true);
        if (GameData.Instance.viveActive)
        {
            text1.CrossFadeAlpha(0, .45f, true);
            text2.CrossFadeAlpha(0, .45f, true);
            text3.CrossFadeAlpha(0, .45f, true);
            text4.CrossFadeAlpha(0, .45f, true);
        }
        else
            textHacker.CrossFadeAlpha(0, .45f, true);

        masterUI.SetActive(true);
        if(GameData.Instance.viveActive)
            hackerUI_VR.SetActive(true);
        else
            hackerUI.SetActive(true);

        Time.timeScale = 0;
        GameData.Instance.isPaused = true;
        transitioning = false;
    }

    void unpauseGame()
    {
        textMaster.CrossFadeAlpha(1, 1, true);
        if (GameData.Instance.viveActive)
        {
            text1.CrossFadeAlpha(1, .45f, true);
            text2.CrossFadeAlpha(1, .45f, true);
            text3.CrossFadeAlpha(1, .45f, true);
            text4.CrossFadeAlpha(1, .45f, true);
        }
        else
            textHacker.CrossFadeAlpha(1, .45f, true);

        if (!GameData.Instance.masterIsReady && !GameData.Instance.hackerIsReady)
        {
            textMaster.text = textHacker.text = text1.text = text2.text = text3.text = text4.text = "pause";
        }
        else if (!GameData.Instance.hackerIsReady || !GameData.Instance.masterIsReady)
        {
            if(!GameData.Instance.masterIsReady)
                textHacker.text = text1.text = text2.text = text3.text = text4.text = "wait for the master";
            else if (!GameData.Instance.hackerIsReady)
                textMaster.text = "wait for the hacker";
        }
        else if(!transitioning)
        {
            transitioning = true;
            StartCoroutine(continueWarning());
        }
    }

    IEnumerator continueWarning()
    {
        masterUI.SetActive(false);
        if (!GameData.Instance.viveActive)
            hackerUI.SetActive(false);

        for (int i = 3; i >= 0; i--)
        {
            textMaster.text = textHacker.text = text1.text = text2.text = text3.text = text4.text = "Game continues in:\n" + i;
            yield return new WaitForSecondsRealtime(1f);
        }
        textMaster.CrossFadeAlpha(0, 1, true);
        textHacker.CrossFadeAlpha(0, 1, true);
        if (GameData.Instance.viveActive)
            hackerUI_VR.SetActive(false);

        Time.timeScale = 1;

        GameData.Instance.isPaused = false;
        transitioning = false;
    }
}
