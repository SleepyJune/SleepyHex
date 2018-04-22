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
    
    void Start()
    {

    }

    public void SetButton(LevelTextAsset level)
    {
        this.level = level;

        levelName.text = level.levelID.ToString();
    }

    public void LoadLevel()
    {
        LevelManager.levelNameToLoad = level.name;
        SceneChanger.ChangeScene("Game");
    }
}
