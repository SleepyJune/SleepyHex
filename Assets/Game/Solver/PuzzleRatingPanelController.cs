using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class PuzzleRatingPanelController : MonoBehaviour
{
    public LevelLoader levelLoader;

    public Toggle[] toggles;
    public int[] difficulties;

    public InputField diffInput;

    public Slider slider;

    float difficulty;

    int bigDifficulty = 1;
    float smallDifficulty = .5f;

    void Start()
    {
        for(int i=0;i<toggles.Length;i++)
        {
            var toggle = toggles[i];

            var num = difficulties[i];

            toggle.onValueChanged.AddListener((isSelected) =>
            {
                if (!isSelected)
                {
                    return;
                }

                SetDifficultyToggle(num);
            });
        }
        
        RefreshDifficultyInput();
    }

    public void RefreshDifficultyInput()
    {
        difficulty = bigDifficulty + smallDifficulty;
        diffInput.text = difficulty.ToString();
    }

    public void ShowCurrentDifficulty()
    {
        var level = levelLoader.GetCurrentLevel();

        if (level != null)
        {
            if (level.difficulty > 0)
            {
                slider.value = level.difficulty % 1;

                var big = (int)Math.Floor(level.difficulty);
                if(big >= 1 && big-1 < toggles.Length)
                {
                    toggles[big - 1].isOn = true;
                }

                RefreshDifficultyInput();
            }
        }
    }

    public void SetDifficultyToggle(int difficulty)
    {
        bigDifficulty = difficulty;
        RefreshDifficultyInput();        
    }

    public void SetSliderDifficulty(float difficulty)
    {
        smallDifficulty = (float)Math.Round(difficulty,1);
        RefreshDifficultyInput();
    }

    public void OnSetButtonPressed()
    {
        float difficulty;
        if(float.TryParse(diffInput.text, out difficulty))
        {
            Scene scene = SceneManager.GetActiveScene();

            if(scene.name == "LevelEditor")
            {
                SetLevelDifficulty2(difficulty);
            }
            else
            {
                SetLevelDifficulty(difficulty);
            }
        }
        else
        {
            Debug.Log("Invalid difficulty");
        }
    }

    public void SetLevelDifficulty2(float difficulty)
    {
        var level = levelLoader.GetCurrentLevel();

        if (level != null)
        {
            level.difficulty = (float)Math.Round(difficulty, 2);

            AutoSetID(level);
        }
    }

    public void SetLevelDifficulty(float difficulty)
    {
        var level = levelLoader.GetCurrentLevel();

        if(level != null)
        {
            level.difficulty = (float)Math.Round(difficulty, 2);

            AutoSetID(level);

            Debug.Log("Set difficulty: " + level.difficulty);

            levelLoader.Save(false, false);
        }
    }

    public void AutoSetID(Level level)
    {
        var filtered = LevelSelector.levelDatabase.Values.Where(other => Math.Floor(other.difficulty) == Math.Floor(level.difficulty)).ToList();
        var levelIds = filtered.Select(o => o.levelID);

        bool changeId = false;

        if (level.levelID == 0 || level.levelID > filtered.Count) //not in the list
        {
            changeId = true;
        }
        else
        {
            var other = filtered.FirstOrDefault(o => o.levelID == level.levelID && o.levelName != level.levelName);
            if (other != null) //same id but different name
            {
                changeId = true;
            }
        }

        if (changeId)
        {
            var unfilled = Enumerable.Range(1, filtered.Count).Except(levelIds).FirstOrDefault();

            if (unfilled != 0)
            {
                level.levelID = unfilled;
            }
            else
            {
                level.levelID = filtered.Count + 1;
            }

            Debug.Log("Changed level id: " + level.levelID);

            levelLoader.SetLevelID(level.levelID);
        }
    }
}
