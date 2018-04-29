using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public Text levelName;
    public GameObject[] stars;
    
    LevelTextAsset level;

    LevelSelector2 levelSelector;
    
    void Start()
    {

    }

    public void SetButton(LevelTextAsset level, LevelSelector2 levelSelector)
    {
        this.level = level;
        this.levelSelector = levelSelector;

        levelName.text = level.levelID.ToString();

        var numStars = Score.GetStoredStars(level.name);

        SetStars(numStars);
    }

    public void SetStars(int numStars)
    {
        for (int i = 0; i < numStars; i++)
        {
            var star = stars[i];
            star.SetActive(true);
        }
    }

    public void LoadLevel()
    {
        //LevelManager.levelNameToLoad = level.name;
        //SceneChanger.ChangeScene("Game");

        levelSelector.LoadLevel(level.name);
    }
}
