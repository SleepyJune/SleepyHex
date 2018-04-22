using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class PuzzleRatingPanelController : MonoBehaviour
{
    public LevelLoader levelLoader;
    
    void Start()
    {
        
    }

    public void SetLevelDifficulty(float difficulty)
    {
        var level = levelLoader.GetCurrentLevel();

        if(level != null)
        {
            level.difficulty = (float)Math.Round(difficulty, 2);

            var filtered = LevelSelector.levelDatabase.Values.Where(other => other.difficulty == level.difficulty).ToList();
            var levelIds = filtered.Select(o => o.levelID);

            bool changeId = false;

            if(level.levelID == 0 || level.levelID > filtered.Count) //not in the list
            {
                changeId = true;
            }
            else
            {
                var other = filtered.FirstOrDefault(o => o.levelID == level.levelID && o.name != level.levelName);
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

            levelLoader.Save(false, false);
        }
    }
}
