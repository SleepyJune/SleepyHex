using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;

    public Animator[] stars;

    void Start()
    {
        if(Score.current != null)
        {
            var score = Score.current;

            scoreText.text = score.points.ToString();

            score.SetStoredStars();

            for (int i = 0; i < score.stars; i++)
            {
                var star = stars[i];
                star.SetBool("isEmpty", false);
            }
        }
    }

    public void NextLevel()
    {
        var current = LevelManager.currentLevel;

        if(current != null)
        {
            var index = LevelSelector.levelListDatabase.FindIndex(level => level.name == current.levelName);
            if(index > -1 && index+1 < LevelSelector.levelListDatabase.Count)
            {
                var next = LevelSelector.levelListDatabase[index + 1];

                LevelManager.levelNameToLoad = next.name;
                SceneChanger.ChangeScene("Game");
            }
        }
    }
}
