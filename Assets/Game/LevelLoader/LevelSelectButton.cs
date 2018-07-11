using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public static HashSet<string> unlockedLevels = new HashSet<string>();

    public Text levelName;
    public GameObject[] stars;

    public GameObject lockIcon;
    
    LevelTextAsset level;

    LevelSelector2 levelSelector;

    Button levelButton;
    
    void Start()
    {
        
    }

    public void SetButton(LevelTextAsset level, LevelSelector2 levelSelector)
    {
        levelButton = GetComponent<Button>();

        this.level = level;
        this.levelSelector = levelSelector;

        levelName.text = level.levelID.ToString();

        var numStars = Score.GetStoredStars(level.levelName);

        SetLock(numStars > 0);
        SetStars(numStars);
    }
        
    public void SetStars(int numStars)
    {
        for (int i = 0; i < numStars; i++)
        {
            var star = stars[i];
            star.SetActive(true);
        }

        if (numStars > 0)
        {
            //set next level active

            LevelTextAsset currentLevel = level;

            for (int i = 0; i < 5; i++)
            {
                LevelTextAsset nextLevel;

                if (LevelSelector.levelDatabase.TryGetValue(currentLevel.nextLevel, out nextLevel))
                {                    
                    LevelSelectButton button;
                    if (levelSelector.buttonDatabase.TryGetValue(currentLevel.nextLevel, out button))
                    {
                        button.SetLock(true);
                    }

                    if (!unlockedLevels.Contains(nextLevel.levelName))
                    {
                        unlockedLevels.Add(nextLevel.levelName);
                    }

                    currentLevel = nextLevel;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void SetLock(bool unlocked = false)
    {
        if (!lockIcon.activeSelf)
        {
            return;
        }

#if UNITY_EDITOR
        if (GameManager.instance.devModeManager.unlockAllLevels)
        {
            SetButtonActive();
            return;
        }
#endif

        if (unlocked)
        {
            SetButtonActive();
            return;
        }

        if (unlockedLevels.Contains(level.levelName))
        {
            SetButtonActive();
            return;
        }


        if (level.levelID == 1)
        {
            SetButtonActive();
            return;
        }

        if (level.previousLevel != null && level.previousLevel != "")
        {
            var previousStars = Score.GetStoredStars(level.previousLevel);

            if (previousStars > 0)
            {
                SetButtonActive();
            }
        }
    }

    public void SetButtonActive()
    {
        levelButton.interactable = true;
        lockIcon.SetActive(false);

        if (levelSelector.highestLevelPlayed != null)
        {
            if (level.levelID > levelSelector.highestLevelPlayed.levelID)
            {
                levelSelector.highestLevelPlayed = level;
                levelSelector.SetCurrentLevel();
            }
        }
        else
        {
            levelSelector.highestLevelPlayed = level;
            levelSelector.SetCurrentLevel();
        }
    }

    public void LoadLevel()
    {
        //LevelManager.levelNameToLoad = level.name;
        //SceneChanger.ChangeScene("Game");

        levelSelector.LoadLevel(level.levelName);
    }
}
