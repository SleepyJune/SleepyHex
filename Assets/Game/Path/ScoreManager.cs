using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Analytics;


public class ScoreManager : MonoBehaviour
{
    public Text scoreText;

    public Text minusText;

    public Animator[] stars;

    public LevelSelector2 levelSelector;

    public DialogWindow scorePanel;

    public AudioSource soundSource;

    public AudioClip[] starSounds;

    public ShareButton shareButton;

    public Score score;
    
    void Start()
    {

    }

    public void SetStars(Score score)
    {
        if (score != null)
        {
            this.score = score;
                        
            scorePanel.Show(); //need this here for reseting all animations first

            scoreText.text = score.points.ToString();

            if (score.level.hasSolution)
            {
                var maxScore = score.level.solution.bestScore;

                var leftOver = score.points - maxScore;

                if(leftOver < 0)
                {
                    minusText.text = leftOver.ToString();
                }
                else
                {
                    minusText.text = "";
                }
            }

            levelSelector.SetButtonStars(score);
            score.SetStoredStars();
            score.IncrementPlayCount();
            
            for (int i = 0; i < score.stars; i++)
            {
                var star = stars[i];
                star.SetBool("isEmpty", false);
            }
            
            soundSource.PlayOneShot(starSounds[score.stars - 1]);
            
            SendLevelCompleteEvent(score);
        }
    }

    private void SendLevelCompleteEvent(Score score)
    {
        Debug.Log("Time: " + score.time);

        Analytics.CustomEvent("LevelComplete", new Dictionary<string, object>()
        {
            { "name",  score.level.levelName},
            { "levelID", score.level.levelID },
            { "difficulty", score.level.GetPuzzleDifficulty().ToString() },
            { "rating", score.level.difficulty },
            { "score", score.points },
            { "stars", score.stars },
            { "time", score.time },
            { "hints", score.hintsUsed },
            { "playCount", score.playCount },
        });
    }

    public void ShareScoreText()
    {
        if(score != null && shareButton != null)
        {
            shareButton.ShareTextScore(score);
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
            var index = LevelSelector.levelListDatabase.FindIndex(level => level.levelName == current.levelName);
            if(index > -1 && index+1 < LevelSelector.levelListDatabase.Count)
            {
                var next = LevelSelector.levelListDatabase[index + 1];

                GameManager.instance.LoadLevel(next.levelName);
                scorePanel.Close();

                //LevelManager.levelNameToLoad = next.name;
                //SceneChanger.ChangeScene("Game");
            }
        }
    }
}
