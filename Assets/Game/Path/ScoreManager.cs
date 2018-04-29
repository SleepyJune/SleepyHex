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

    public LevelSelector2 levelSelector;

    public DialogWindow scorePanel;

    public AudioSource soundSource;

    public AudioClip[] starSounds;

    void Start()
    {

    }

    public void SetStars(Score score)
    {
        if (score != null)
        {
            scorePanel.Show(); //need this here for reseting all animations first

            scoreText.text = score.points.ToString();

            levelSelector.SetButtonStars(score);
            score.SetStoredStars();
            
            for (int i = 0; i < score.stars; i++)
            {
                var star = stars[i];
                star.SetBool("isEmpty", false);
            }
            
            soundSource.PlayOneShot(starSounds[score.stars - 1]);                        
        }
    }

    public void Restart()
    {
        var current = LevelManager.currentLevel;

        if (current != null)
        {
            GameManager.instance.LoadLevel(current.levelName);
            scorePanel.Close();
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

                GameManager.instance.LoadLevel(next.name);
                scorePanel.Close();

                //LevelManager.levelNameToLoad = next.name;
                //SceneChanger.ChangeScene("Game");
            }
        }
    }
}
