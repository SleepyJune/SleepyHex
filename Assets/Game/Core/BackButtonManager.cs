using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonManager : MonoBehaviour
{
    public DialogueGroup dialogueGroup;

    public DialogWindow exitGameDialogue;

    bool isGameScene = false;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            isGameScene = true;
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    void HomeSceneBackButton()
    {
        if (exitGameDialogue)
        {
            if (exitGameDialogue.panel.activeSelf)
            {
                exitGameDialogue.Close();
            }
            else
            {
                exitGameDialogue.Show();
            }
        }
    }

    void GameSceneBackButton()
    {
        var curr = dialogueGroup.GetCurrentWindow();

        Debug.Log(curr);

        if (curr == "TutorialOverlay" || curr == "Settings" || curr == "TutorialParent" 
            || curr == "DailyBonusManager" || curr == "MessageRateUs")
        {
            dialogueGroup.CloseWindow();
        }
        else if (curr == "Game" && GameManager.instance.pathManager.canFillSlots)
        {
            dialogueGroup.ShowWindow("Settings");
        }
        else if (curr == "Score")
        {
            if (GameManager.instance && GameManager.instance.scoreManager)
            {
                GameManager.instance.scoreManager.Restart();
            }
        }
        else if (curr == "LevelSelect")
        {
            dialogueGroup.SetActive("LevelDifficulty");
        }
        else if (curr == "LevelDifficulty")
        {
            SceneChanger.ChangeScene("Home");
        }
        else
        {

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameScene)
            {
                GameSceneBackButton();
            }
            else
            {
                HomeSceneBackButton();
            }
        }
    }
}
