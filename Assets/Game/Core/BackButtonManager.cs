using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonManager : MonoBehaviour
{
    public DialogueGroup dialogueGroup;

    bool isGameScene = false;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            isGameScene = true;
        }
    }

    void HomeSceneBackButton()
    {
        Application.Quit();
    }

    void GameSceneBackButton()
    {
        var curr = dialogueGroup.currentActiveWindow;

        Debug.Log(curr);

        if (curr == "Game")
        {
            dialogueGroup.ShowWindow("Settings");
        }
        else if (curr == "Settings")
        {
            dialogueGroup.CloseWindow("Game");
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
