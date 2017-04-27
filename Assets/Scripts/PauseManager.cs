using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    public Text textMaster;
    public GameObject masterUI;

    public Text textHacker;
    public GameObject hackerUI;

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
        else if(Input.GetKeyDown(KeyCode.Escape))
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
        GameData.Instance.hackerIsReady = true;
    }

    void pauseGame()
    {
        GameData.Instance.hackerIsReady = false;
        GameData.Instance.masterIsReady = false;
        StartCoroutine(pauseWarning());
    }

    IEnumerator pauseWarning()
    {
        for (int i = 3; i >= 0; i--)
        {
            textMaster.text = textHacker.text = "Game is going to pause in:\n" + i;
            textMaster.CrossFadeAlpha(.75f, .45f, true);
            textHacker.CrossFadeAlpha(.75f, .45f, true);
            yield return new WaitForSeconds(.5f);
            textMaster.CrossFadeAlpha(1, .45f, true);
            textHacker.CrossFadeAlpha(1, .45f, true);
            yield return new WaitForSeconds(.5f);
        }
        textMaster.CrossFadeAlpha(0, 1, true);
        textHacker.CrossFadeAlpha(0, 1, true);
        masterUI.SetActive(true);
        hackerUI.SetActive(true);
        Time.timeScale = 0;
        GameData.Instance.isPaused = true;
    }

    void unpauseGame()
    {
        textMaster.CrossFadeAlpha(1, 1, true);
        textHacker.CrossFadeAlpha(1, 1, true);
        if (!GameData.Instance.masterIsReady && !GameData.Instance.hackerIsReady)
        {
            textMaster.text = textHacker.text = "pause";
        }
        else if (!GameData.Instance.hackerIsReady || !GameData.Instance.masterIsReady)
        {
            if(!GameData.Instance.masterIsReady)
                textHacker.text = "wait for the master";
            else if (!GameData.Instance.hackerIsReady)
                textMaster.text = "wait for the hacker";
        }
        else
        {
            masterUI.SetActive(false);
            hackerUI.SetActive(false);
            textMaster.CrossFadeAlpha(0, 1, true);
            textHacker.CrossFadeAlpha(0, 1, true);
            Time.timeScale = 1;
            GameData.Instance.isPaused = false;
        }
    }
}
